"use client";

import { useCallback, useEffect, useRef, useState } from "react";
import { useSearchParams } from "next/navigation";
import Link from "next/link";
import { MessageCircle, Paperclip, Search, Send, Smile } from "lucide-react";
import { socialApi } from "@/lib/services";
import { resolvePhotoUrl } from "@/lib/media";
import { useAuth } from "@/context/AuthContext";
import type { Conversation, Message } from "@/lib/types";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { EmptyState, LoadingSpinner } from "@/components/ui/LoadingSpinner";

function formatTime(dateStr: string) {
  const d = new Date(dateStr);
  const now = new Date();
  const isToday = d.toDateString() === now.toDateString();
  if (isToday) return d.toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });
  return d.toLocaleDateString([], { month: "short", day: "numeric" });
}

function formatMessageTime(dateStr: string) {
  return new Date(dateStr).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });
}

export default function MessagesPage() {
  const params = useSearchParams();
  const { user } = useAuth();
  const [conversations, setConversations] = useState<Conversation[]>([]);
  const [selected, setSelected] = useState<string | null>(params.get("user"));
  const [messages, setMessages] = useState<Message[]>([]);
  const [text, setText] = useState("");
  const [search, setSearch] = useState("");
  const [loading, setLoading] = useState(true);
  const [sending, setSending] = useState(false);
  const bottomRef = useRef<HTMLDivElement>(null);

  const loadConversations = useCallback(async () => {
    setConversations(await socialApi.conversations());
    setLoading(false);
  }, []);

  const loadMessages = useCallback(async (userId: string) => {
    setMessages(await socialApi.messages(userId));
  }, []);

  useEffect(() => { loadConversations(); }, [loadConversations]);
  useEffect(() => { if (selected) loadMessages(selected); }, [selected, loadMessages]);
  useEffect(() => { bottomRef.current?.scrollIntoView({ behavior: "smooth" }); }, [messages]);

  const send = async () => {
    if (!selected || !text.trim() || sending) return;
    setSending(true);
    try {
      await socialApi.sendMessage(selected, text.trim());
      setText("");
      await loadMessages(selected);
      await loadConversations();
    } finally {
      setSending(false);
    }
  };

  const active = conversations.find((c) => c.userId === selected);
  const filtered = conversations.filter((c) =>
    c.name.toLowerCase().includes(search.toLowerCase())
  );

  return (
    <div className="flex h-[calc(100vh-8rem)] flex-col">
      <div className="mb-4">
        <h1 className="text-2xl font-bold text-slate-900">Messages</h1>
        <p className="text-sm text-slate-500">Chat with your matches in real time</p>
      </div>

      <div className="flex min-h-0 flex-1 overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-lg">
        {/* Sidebar */}
        <div className={`flex w-full flex-col border-r border-slate-100 bg-slate-50/50 md:w-80 ${selected ? "hidden md:flex" : "flex"}`}>
          <div className="border-b border-slate-100 p-4">
            <div className="relative">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400" size={16} />
              <input
                value={search}
                onChange={(e) => setSearch(e.target.value)}
                placeholder="Search conversations..."
                className="w-full rounded-xl border border-slate-200 bg-white py-2.5 pl-9 pr-3 text-sm outline-none focus:border-rose-300 focus:ring-2 focus:ring-rose-100"
              />
            </div>
          </div>
          <div className="flex-1 overflow-y-auto">
            {loading ? (
              <div className="p-6"><LoadingSpinner /></div>
            ) : filtered.length === 0 ? (
              <EmptyState message="No conversations yet. Match with someone from Discover!" />
            ) : (
              filtered.map((c) => {
                const photo = resolvePhotoUrl(c.photoUrl);
                const isActive = selected === c.userId;
                return (
                  <button
                    key={c.userId}
                    type="button"
                    onClick={() => setSelected(c.userId)}
                    className={`flex w-full items-center gap-3 border-b border-slate-50 px-4 py-3 text-left transition hover:bg-white ${isActive ? "bg-white shadow-sm" : ""}`}
                  >
                    {photo ? (
                      // eslint-disable-next-line @next/next/no-img-element
                      <img src={photo} alt="" className="h-12 w-12 shrink-0 rounded-full object-cover ring-2 ring-white" />
                    ) : (
                      <div className="flex h-12 w-12 shrink-0 items-center justify-center rounded-full bg-gradient-to-br from-rose-400 to-rose-600 text-sm font-bold text-white">
                        {c.name.charAt(0)}
                      </div>
                    )}
                    <div className="min-w-0 flex-1">
                      <div className="flex items-center justify-between gap-2">
                        <p className="truncate font-semibold text-slate-900">{c.name}</p>
                        {c.lastMessageOn && (
                          <span className="shrink-0 text-[10px] text-slate-400">{formatTime(c.lastMessageOn)}</span>
                        )}
                      </div>
                      <div className="flex items-center justify-between gap-2">
                        <p className="truncate text-xs text-slate-500">{c.lastMessage || "Start chatting..."}</p>
                        {c.unreadCount > 0 && (
                          <span className="flex h-5 min-w-5 shrink-0 items-center justify-center rounded-full bg-rose-500 px-1.5 text-[10px] font-bold text-white">
                            {c.unreadCount}
                          </span>
                        )}
                      </div>
                    </div>
                  </button>
                );
              })
            )}
          </div>
        </div>

        {/* Chat area */}
        <div className={`flex min-w-0 flex-1 flex-col ${!selected ? "hidden md:flex" : "flex"}`}>
          {!selected ? (
            <div className="flex flex-1 flex-col items-center justify-center gap-3 bg-gradient-to-b from-rose-50/30 to-white p-8 text-center">
              <div className="rounded-full bg-rose-100 p-4">
                <MessageCircle className="text-rose-500" size={40} />
              </div>
              <h3 className="text-lg font-semibold text-slate-800">Select a conversation</h3>
              <p className="max-w-sm text-sm text-slate-500">
                Choose a match from the list to start chatting. You can only message users you have matched with.
              </p>
              <Link href="/matches" className="text-sm font-medium text-rose-600 hover:underline">
                View your matches →
              </Link>
            </div>
          ) : (
            <>
              {/* Chat header */}
              <div className="flex items-center gap-3 border-b border-slate-100 bg-white px-4 py-3">
                <button
                  type="button"
                  className="text-sm text-rose-600 md:hidden"
                  onClick={() => setSelected(null)}
                >
                  ← Back
                </button>
                {resolvePhotoUrl(active?.photoUrl) ? (
                  // eslint-disable-next-line @next/next/no-img-element
                  <img src={resolvePhotoUrl(active?.photoUrl)} alt="" className="h-10 w-10 rounded-full object-cover" />
                ) : (
                  <div className="flex h-10 w-10 items-center justify-center rounded-full bg-rose-500 text-sm font-bold text-white">
                    {active?.name.charAt(0)}
                  </div>
                )}
                <div className="flex-1">
                  <p className="font-semibold text-slate-900">{active?.name}</p>
                  <p className="text-xs text-green-600">Online</p>
                </div>
                <Link href={`/members/${selected}`}>
                  <Button variant="ghost" size="sm">View profile</Button>
                </Link>
              </div>

              {/* Messages */}
              <div className="flex-1 overflow-y-auto bg-[#f0f2f5] px-4 py-4">
                <div className="mx-auto max-w-2xl space-y-3">
                  {messages.map((m) => {
                    const isMine = m.senderUserId === user?.userId;
                    return (
                      <div key={m.messageId} className={`flex ${isMine ? "justify-end" : "justify-start"}`}>
                        <div
                          className={`relative max-w-[75%] rounded-2xl px-4 py-2.5 shadow-sm ${
                            isMine
                              ? "rounded-br-md bg-gradient-to-br from-rose-500 to-rose-600 text-white"
                              : "rounded-bl-md bg-white text-slate-800"
                          }`}
                        >
                          <p className="text-sm leading-relaxed">{m.message}</p>
                          <p className={`mt-1 text-right text-[10px] ${isMine ? "text-rose-100" : "text-slate-400"}`}>
                            {formatMessageTime(m.sentOn)}
                          </p>
                        </div>
                      </div>
                    );
                  })}
                  <div ref={bottomRef} />
                </div>
              </div>

              {/* Input */}
              <div className="border-t border-slate-100 bg-white p-4">
                <div className="mx-auto flex max-w-2xl items-end gap-2">
                  <button type="button" className="rounded-full p-2 text-slate-400 hover:bg-slate-100 hover:text-slate-600">
                    <Smile size={20} />
                  </button>
                  <button type="button" className="rounded-full p-2 text-slate-400 hover:bg-slate-100 hover:text-slate-600">
                    <Paperclip size={20} />
                  </button>
                  <div className="flex-1">
                    <Input
                      value={text}
                      onChange={(e) => setText(e.target.value)}
                      onKeyDown={(e) => { if (e.key === "Enter" && !e.shiftKey) { e.preventDefault(); send(); } }}
                      placeholder="Type a message..."
                      className="rounded-2xl border-slate-200 bg-slate-50"
                    />
                  </div>
                  <button
                    type="button"
                    onClick={send}
                    disabled={!text.trim() || sending}
                    className="flex h-11 w-11 shrink-0 items-center justify-center rounded-full bg-gradient-to-br from-rose-500 to-rose-600 text-white shadow-md transition hover:from-rose-600 hover:to-rose-700 disabled:opacity-50"
                  >
                    <Send size={18} />
                  </button>
                </div>
              </div>
            </>
          )}
        </div>
      </div>
    </div>
  );
}
