import { useEffect, useRef, useCallback } from 'react';
import { createPortal } from 'react-dom';
import { Bell } from 'lucide-react';
import { useSidebar } from '@/hooks/useSidebar';

interface NotificationDropdownProps {
  open: boolean;
  onToggle: () => void;
  onClose: () => void;
  triggerRef: React.RefObject<HTMLButtonElement>;
}

export function NotificationDropdown({ open, onToggle, onClose, triggerRef }: NotificationDropdownProps) {
  const contentRef = useRef<HTMLDivElement>(null);
  const { collapsed } = useSidebar();

  const handleClose = useCallback(() => {
    onClose();
    triggerRef.current?.focus();
  }, [onClose, triggerRef]);

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
  }, [open, handleClose, triggerRef]);

  return (
    <>
      <button
        ref={triggerRef}
        onClick={onToggle}
        aria-label="Notifications"
        aria-expanded={open}
        className="sidebar-item w-full relative"
      >
        <Bell size={20} className="shrink-0" />
        <span
          className="overflow-hidden whitespace-nowrap transition-all duration-200"
          style={{ width: collapsed ? 0 : 'auto', opacity: collapsed ? 0 : 1 }}
        >
          Notifications
        </span>
        {/* Badge */}
        <span
          className="absolute flex h-4 min-w-4 items-center justify-center rounded-full bg-nebula-violet text-[10px] font-bold text-white"
          style={{
            top: '0.25rem',
            left: collapsed ? '1.5rem' : 'auto',
            right: collapsed ? 'auto' : '0.75rem',
          }}
        >
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
                ? triggerRef.current.getBoundingClientRect().top
                : 0,
              left: triggerRef.current
                ? triggerRef.current.getBoundingClientRect().right + 8
                : 0,
              minWidth: 280,
            }}
          >
            <h3 className="text-sm font-semibold" style={{ color: 'var(--text-primary)' }}>
              Notifications
            </h3>
            <p className="mt-3 text-xs" style={{ color: 'var(--text-muted)' }}>
              No new notifications
            </p>
          </div>,
          document.body,
        )}
    </>
  );
}
