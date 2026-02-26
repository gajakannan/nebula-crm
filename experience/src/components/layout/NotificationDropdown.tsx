import { useEffect, useRef, useCallback } from 'react';
import { createPortal } from 'react-dom';
import { Bell } from 'lucide-react';

interface NotificationDropdownProps {
  open: boolean;
  onToggle: () => void;
  onClose: () => void;
}

export function NotificationDropdown({ open, onToggle, onClose }: NotificationDropdownProps) {
  const triggerRef = useRef<HTMLButtonElement>(null!);
  const contentRef = useRef<HTMLDivElement>(null);

  const handleClose = useCallback(() => {
    onClose();
    triggerRef.current?.focus();
  }, [onClose]);

  useEffect(() => {
    if (!open) return;

    function handleKeyDown(e: KeyboardEvent) {
      if (e.key === 'Escape') handleClose();
    }

    function handleClickOutside(e: MouseEvent) {
      if (
        contentRef.current &&
        !contentRef.current.contains(e.target as Node) &&
        triggerRef.current &&
        !triggerRef.current.contains(e.target as Node)
      ) {
        handleClose();
      }
    }

    document.addEventListener('keydown', handleKeyDown);
    document.addEventListener('mousedown', handleClickOutside);
    return () => {
      document.removeEventListener('keydown', handleKeyDown);
      document.removeEventListener('mousedown', handleClickOutside);
    };
  }, [open, handleClose]);

  return (
    <>
      <button
        ref={triggerRef}
        onClick={onToggle}
        aria-label="Notifications"
        aria-expanded={open}
        className="relative flex h-9 w-9 items-center justify-center rounded-md text-text-secondary transition-colors"
      >
        <Bell size={20} />
        {/* Badge */}
        <span className="absolute -top-0.5 -right-0.5 flex h-4 min-w-4 items-center justify-center rounded-full bg-nebula-violet text-[10px] font-bold text-white">
          3
        </span>
      </button>
      {open &&
        createPortal(
          <div
            ref={contentRef}
            className="fixed z-50 glass-card rounded-xl p-4 shadow-2xl"
            style={{
              top: triggerRef.current
                ? triggerRef.current.getBoundingClientRect().bottom + 8
                : 0,
              right: triggerRef.current
                ? window.innerWidth - triggerRef.current.getBoundingClientRect().right
                : 0,
              minWidth: 280,
            }}
          >
            <h3 className="text-sm font-semibold text-text-primary">
              Notifications
            </h3>
            <p className="mt-3 text-xs text-text-muted">
              No new notifications
            </p>
          </div>,
          document.body,
        )}
    </>
  );
}
