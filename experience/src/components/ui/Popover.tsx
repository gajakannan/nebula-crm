import { useEffect, useRef, useState, useCallback } from 'react';
import { createPortal } from 'react-dom';
import { cn } from '@/lib/utils';

interface PopoverProps {
  trigger: React.ReactNode;
  children: React.ReactNode;
  className?: string;
}

export function Popover({ trigger, children, className }: PopoverProps) {
  const [open, setOpen] = useState(false);
  const triggerRef = useRef<HTMLDivElement>(null);
  const contentRef = useRef<HTMLDivElement>(null);

  const close = useCallback(() => setOpen(false), []);

  useEffect(() => {
    if (!open) return;

    function handleKeyDown(e: KeyboardEvent) {
      if (e.key === 'Escape') close();
    }

    function handleClickOutside(e: MouseEvent) {
      if (
        contentRef.current &&
        !contentRef.current.contains(e.target as Node) &&
        triggerRef.current &&
        !triggerRef.current.contains(e.target as Node)
      ) {
        close();
      }
    }

    document.addEventListener('keydown', handleKeyDown);
    document.addEventListener('mousedown', handleClickOutside);
    return () => {
      document.removeEventListener('keydown', handleKeyDown);
      document.removeEventListener('mousedown', handleClickOutside);
    };
  }, [open, close]);

  const triggerRect = triggerRef.current?.getBoundingClientRect();

  return (
    <>
      <div
        ref={triggerRef}
        onClick={() => setOpen((prev) => !prev)}
        className="cursor-pointer"
      >
        {trigger}
      </div>
      {open &&
        createPortal(
          <div
            ref={contentRef}
            className={cn(
              'absolute z-50 glass-card rounded-xl p-4 shadow-2xl',
              className,
            )}
            style={{
              top: (triggerRect?.bottom ?? 0) + 8 + window.scrollY,
              left: triggerRect?.left ?? 0,
              minWidth: 280,
            }}
          >
            {children}
          </div>,
          document.body,
        )}
    </>
  );
}
