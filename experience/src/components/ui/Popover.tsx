import React, { useEffect, useRef, useState, useCallback } from 'react';
import { createPortal } from 'react-dom';
import { cn } from '@/lib/utils';

interface PopoverProps {
  trigger: React.ReactNode;
  children: React.ReactNode;
  className?: string;
}

export function Popover({ trigger, children, className }: PopoverProps) {
  const [open, setOpen] = useState(false);
  const triggerRef = useRef<HTMLElement>(null);
  const contentRef = useRef<HTMLDivElement>(null);
  const [rect, setRect] = useState<DOMRect | null>(null);

  const close = useCallback(() => setOpen(false), []);

  const updatePosition = useCallback(() => {
    if (triggerRef.current) {
      setRect(triggerRef.current.getBoundingClientRect());
    }
  }, []);

  const handleToggle = useCallback(() => {
    setOpen((prev) => {
      if (!prev && triggerRef.current) {
        setRect(triggerRef.current.getBoundingClientRect());
      }
      return !prev;
    });
  }, []);

  useEffect(() => {
    if (!open) return;

    updatePosition();

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
    window.addEventListener('resize', updatePosition);
    window.addEventListener('scroll', updatePosition, true);
    return () => {
      document.removeEventListener('keydown', handleKeyDown);
      document.removeEventListener('mousedown', handleClickOutside);
      window.removeEventListener('resize', updatePosition);
      window.removeEventListener('scroll', updatePosition, true);
    };
  }, [open, close, updatePosition]);

  // Attach ref + onClick directly to the trigger element (no wrapper div)
  const clonedTrigger = React.isValidElement(trigger)
    ? React.cloneElement(trigger as React.ReactElement<Record<string, unknown>>, {
        ref: triggerRef,
        onClick: (e: React.MouseEvent) => {
          const originalOnClick = (trigger as React.ReactElement<Record<string, unknown>>).props
            .onClick as ((e: React.MouseEvent) => void) | undefined;
          originalOnClick?.(e);
          handleToggle();
        },
        className: cn(
          (trigger as React.ReactElement<Record<string, unknown>>).props.className as string | undefined,
          'cursor-pointer',
        ),
      })
    : (
        <div ref={triggerRef as React.RefObject<HTMLDivElement>} onClick={handleToggle} className="cursor-pointer">
          {trigger}
        </div>
      );

  return (
    <>
      {clonedTrigger}
      {open &&
        rect &&
        createPortal(
          <div
            ref={contentRef}
            className={cn(
              'fixed z-50 glass-card rounded-xl p-4 shadow-2xl',
              className,
            )}
            style={{
              top: rect.bottom + 8,
              left: rect.left,
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
