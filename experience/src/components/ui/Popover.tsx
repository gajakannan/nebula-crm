import React, { useEffect, useRef, useState, useCallback, useId } from 'react';
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
  const contentId = useId();

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

  const isNativeInteractiveElement = (node: React.ReactElement<Record<string, unknown>>) =>
    typeof node.type === 'string' && ['button', 'a', 'input', 'select', 'textarea'].includes(node.type);

  // Attach ref + handlers directly to the trigger element (no wrapper div)
  const clonedTrigger = React.isValidElement(trigger)
    ? React.cloneElement(trigger as React.ReactElement<Record<string, unknown>>, {
        ref: triggerRef,
        'aria-haspopup': 'dialog',
        'aria-expanded': open,
        'aria-controls': open ? contentId : undefined,
        ...(isNativeInteractiveElement(trigger as React.ReactElement<Record<string, unknown>>)
          ? {}
          : { role: 'button', tabIndex: 0 }),
        onClick: (e: React.MouseEvent) => {
          const originalOnClick = (trigger as React.ReactElement<Record<string, unknown>>).props
            .onClick as ((e: React.MouseEvent) => void) | undefined;
          originalOnClick?.(e);
          handleToggle();
        },
        onKeyDown: (e: React.KeyboardEvent) => {
          const originalOnKeyDown = (trigger as React.ReactElement<Record<string, unknown>>).props
            .onKeyDown as ((e: React.KeyboardEvent) => void) | undefined;
          originalOnKeyDown?.(e);
          if (!isNativeInteractiveElement(trigger as React.ReactElement<Record<string, unknown>>) && (e.key === 'Enter' || e.key === ' ')) {
            e.preventDefault();
            handleToggle();
          }
        },
        className: cn(
          (trigger as React.ReactElement<Record<string, unknown>>).props.className as string | undefined,
          'cursor-pointer',
        ),
      })
    : (
        <button
          ref={triggerRef as React.RefObject<HTMLButtonElement>}
          type="button"
          aria-haspopup="dialog"
          aria-expanded={open}
          aria-controls={open ? contentId : undefined}
          onClick={handleToggle}
          className="cursor-pointer"
        >
          {trigger}
        </button>
      );

  return (
    <>
      {clonedTrigger}
      {open &&
        rect &&
        createPortal(
          <div
            id={contentId}
            ref={contentRef}
            role="dialog"
            aria-modal="false"
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
