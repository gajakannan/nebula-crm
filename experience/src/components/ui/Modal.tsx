import { useEffect, useRef, useCallback } from 'react';
import { createPortal } from 'react-dom';
import { cn } from '@/lib/utils';

interface ModalProps {
  open: boolean;
  onClose: () => void;
  title: string;
  children: React.ReactNode;
  className?: string;
}

export function Modal({ open, onClose, title, children, className }: ModalProps) {
  const contentRef = useRef<HTMLDivElement>(null);

  const handleClose = useCallback(() => onClose(), [onClose]);

  useEffect(() => {
    if (!open) return;

    function handleKeyDown(e: KeyboardEvent) {
      if (e.key === 'Escape') handleClose();
    }

    document.addEventListener('keydown', handleKeyDown);
    document.body.style.overflow = 'hidden';
    return () => {
      document.removeEventListener('keydown', handleKeyDown);
      document.body.style.overflow = '';
    };
  }, [open, handleClose]);

  function handleBackdropClick(e: React.MouseEvent) {
    if (contentRef.current && !contentRef.current.contains(e.target as Node)) {
      handleClose();
    }
  }

  if (!open) return null;

  return createPortal(
    <div
      className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 backdrop-blur-md"
      onClick={handleBackdropClick}
    >
      <div
        ref={contentRef}
        className={cn(
          'mx-4 w-full max-w-lg rounded-xl glass-card shadow-2xl',
          className,
        )}
      >
        <div className="flex items-center justify-between border-b border-surface-border px-5 py-4">
          <h2 className="text-sm font-semibold text-text-primary">{title}</h2>
          <button
            onClick={handleClose}
            className="rounded-md p-1 text-text-secondary transition-colors hover:bg-surface-card-hover hover:text-text-primary"
          >
            <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
              <path strokeLinecap="round" strokeLinejoin="round" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>
        <div className="p-5">{children}</div>
      </div>
    </div>,
    document.body,
  );
}
