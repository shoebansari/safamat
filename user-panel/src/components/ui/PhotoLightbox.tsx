"use client";

import { useCallback, useEffect, useState } from "react";
import { ChevronLeft, ChevronRight, X } from "lucide-react";
import { resolvePhotoUrl } from "@/lib/media";
import { DefaultAvatar } from "@/components/ui/DefaultAvatar";

interface PhotoLightboxProps {
  photos: string[];
  initialIndex?: number;
  alt?: string;
  onClose: () => void;
}

export function PhotoLightbox({ photos, initialIndex = 0, alt = "Photo", onClose }: PhotoLightboxProps) {
  const [index, setIndex] = useState(initialIndex);
  const resolved = photos.map(resolvePhotoUrl).filter(Boolean);

  const prev = useCallback(() => {
    setIndex((i) => (i > 0 ? i - 1 : resolved.length - 1));
  }, [resolved.length]);

  const next = useCallback(() => {
    setIndex((i) => (i < resolved.length - 1 ? i + 1 : 0));
  }, [resolved.length]);

  useEffect(() => {
    const onKey = (e: KeyboardEvent) => {
      if (e.key === "Escape") onClose();
      if (e.key === "ArrowLeft") prev();
      if (e.key === "ArrowRight") next();
    };
    document.body.style.overflow = "hidden";
    window.addEventListener("keydown", onKey);
    return () => {
      document.body.style.overflow = "";
      window.removeEventListener("keydown", onKey);
    };
  }, [onClose, prev, next]);

  if (resolved.length === 0) return null;

  return (
    <div
      className="fixed inset-0 z-50 flex items-center justify-center bg-black/90 p-4"
      onClick={onClose}
      role="dialog"
      aria-modal="true"
    >
      <button
        type="button"
        onClick={onClose}
        className="absolute right-4 top-4 rounded-full bg-white/10 p-2 text-white hover:bg-white/20"
        aria-label="Close"
      >
        <X size={24} />
      </button>

      {resolved.length > 1 && (
        <>
          <button
            type="button"
            onClick={(e) => { e.stopPropagation(); prev(); }}
            className="absolute left-4 top-1/2 -translate-y-1/2 rounded-full bg-white/10 p-3 text-white hover:bg-white/20"
            aria-label="Previous photo"
          >
            <ChevronLeft size={28} />
          </button>
          <button
            type="button"
            onClick={(e) => { e.stopPropagation(); next(); }}
            className="absolute right-4 top-1/2 -translate-y-1/2 rounded-full bg-white/10 p-3 text-white hover:bg-white/20"
            aria-label="Next photo"
          >
            <ChevronRight size={28} />
          </button>
        </>
      )}

      <div className="relative max-h-[85vh] max-w-4xl" onClick={(e) => e.stopPropagation()}>
        {/* eslint-disable-next-line @next/next/no-img-element */}
        <img
          src={resolved[index]}
          alt={`${alt} ${index + 1}`}
          className="max-h-[85vh] w-auto max-w-full rounded-lg object-contain shadow-2xl"
        />
        {resolved.length > 1 && (
          <p className="mt-3 text-center text-sm text-white/80">
            {index + 1} / {resolved.length}
          </p>
        )}
      </div>
    </div>
  );
}

interface ClickablePhotoProps {
  src?: string | null;
  photos?: string[];
  alt?: string;
  className?: string;
  fallbackClassName?: string;
}

export function ClickablePhoto({ src, photos, alt = "", className = "", fallbackClassName = "" }: ClickablePhotoProps) {
  const [open, setOpen] = useState(false);
  const [imgError, setImgError] = useState(false);
  const gallery = photos?.length ? photos : src ? [src] : [];
  const resolved = resolvePhotoUrl(src);
  const displaySrc = resolved || resolvePhotoUrl(gallery[0]);

  if ((!displaySrc || imgError) && gallery.length === 0) {
    return <DefaultAvatar name={alt} className={fallbackClassName || className} />;
  }

  return (
    <>
      <button type="button" onClick={() => displaySrc && !imgError && setOpen(true)} className="block w-full cursor-pointer">
        {displaySrc && !imgError ? (
          // eslint-disable-next-line @next/next/no-img-element
          <img
            src={displaySrc}
            alt={alt}
            className={`${className} transition hover:opacity-95`}
            onError={() => setImgError(true)}
          />
        ) : (
          <DefaultAvatar name={alt} className={fallbackClassName || className} />
        )}
      </button>
      {open && gallery.length > 0 && (
        <PhotoLightbox photos={gallery} alt={alt} onClose={() => setOpen(false)} />
      )}
    </>
  );
}
