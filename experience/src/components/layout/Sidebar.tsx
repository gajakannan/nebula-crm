import { useRef, useState, useEffect } from 'react';
import { Link, useLocation } from 'react-router-dom';
import {
  LayoutDashboard,
  Users,
  PanelLeftClose,
  PanelLeft,
  Sun,
  Moon,
  LogOut,
  ChevronRight,
} from 'lucide-react';
import { cn } from '@/lib/utils';
import { useSidebar } from '@/hooks/useSidebar';
import { useTheme } from '@/hooks/useTheme';
import { NotificationDropdown } from './NotificationDropdown';

const NAV_ITEMS = [
  { label: 'Dashboard', href: '/', icon: LayoutDashboard },
  { label: 'Brokers', href: '/brokers', icon: Users },
];

function isActive(href: string, pathname: string) {
  return href === '/' ? pathname === '/' : pathname.startsWith(href);
}

export function Sidebar() {
  const { collapsed, toggleCollapsed, mobileOpen, closeMobile } = useSidebar();
  const { theme, toggleTheme } = useTheme();
  const { pathname } = useLocation();
  const [notifOpen, setNotifOpen] = useState(false);
  const bellRef = useRef<HTMLButtonElement>(null!);


  // Close mobile sidebar on route change
  useEffect(() => {
    closeMobile();
  }, [pathname, closeMobile]);

  const sidebarWidth = collapsed ? 'w-16' : 'w-64';

  const sidebarContent = (
    <div className={cn('sidebar h-full flex flex-col', sidebarWidth)}>
      {/* Header: logo + collapse toggle */}
      <div className="flex h-14 items-center justify-between px-3">
        <Link to="/" className="flex items-center gap-2 overflow-hidden">
          <span className="flex h-8 w-8 shrink-0 items-center justify-center rounded-lg bg-nebula-violet/20 text-nebula-violet font-bold text-sm">
            N
          </span>
          <span
            className="overflow-hidden whitespace-nowrap transition-all duration-200"
            style={{ width: collapsed ? 0 : 'auto', opacity: collapsed ? 0 : 1 }}
          >
            <span className="text-lg font-bold tracking-tight text-nebula-violet drop-shadow-[0_0_8px_rgba(139,92,246,0.3)]">
              Nebula
            </span>
            <span className="ml-1 text-sm" style={{ color: 'var(--text-muted)' }}>CRM</span>
          </span>
        </Link>
        <button
          onClick={toggleCollapsed}
          aria-label={collapsed ? 'Expand sidebar' : 'Collapse sidebar'}
          aria-expanded={!collapsed}
          className="hidden lg:flex h-7 w-7 shrink-0 items-center justify-center rounded-md transition-colors"
          style={{ color: 'var(--text-muted)' }}
        >
          {collapsed ? <PanelLeft size={16} /> : <PanelLeftClose size={16} />}
        </button>
      </div>

      {/* Notifications */}
      <div className="px-2 py-1">
        <NotificationDropdown
          open={notifOpen}
          onToggle={() => setNotifOpen((p) => !p)}
          onClose={() => setNotifOpen(false)}
          triggerRef={bellRef}
        />
      </div>

      {/* Navigation */}
      <div className="px-2 pt-4">
        {!collapsed && (
          <p className="mb-2 px-3 text-[10px] font-semibold uppercase tracking-widest"
            style={{ color: 'var(--text-muted)' }}
          >
            Navigation
          </p>
        )}
        <nav aria-label="Main navigation" className="space-y-1">
          {NAV_ITEMS.map((item) => {
            const active = isActive(item.href, pathname);
            return (
              <Link
                key={item.href}
                to={item.href}
                aria-current={active ? 'page' : undefined}
                className={cn('sidebar-item', active && 'sidebar-item-active')}
              >
                <item.icon size={20} className="shrink-0" />
                <span
                  className="overflow-hidden whitespace-nowrap transition-all duration-200"
                  style={{ width: collapsed ? 0 : 'auto', opacity: collapsed ? 0 : 1 }}
                >
                  {item.label}
                </span>
                {active && !collapsed && (
                  <ChevronRight size={14} className="ml-auto shrink-0 opacity-50" />
                )}
              </Link>
            );
          })}
        </nav>
      </div>

      {/* Spacer */}
      <div className="flex-1" />

      {/* Theme toggle */}
      <div className="px-2 pb-1">
        <button
          onClick={toggleTheme}
          className="sidebar-item w-full"
          aria-label={theme === 'dark' ? 'Switch to light mode' : 'Switch to dark mode'}
        >
          {theme === 'dark' ? (
            <Sun size={20} className="shrink-0" />
          ) : (
            <Moon size={20} className="shrink-0" />
          )}
          <span
            className="overflow-hidden whitespace-nowrap transition-all duration-200"
            style={{ width: collapsed ? 0 : 'auto', opacity: collapsed ? 0 : 1 }}
          >
            {theme === 'dark' ? 'Light mode' : 'Dark mode'}
          </span>
        </button>
      </div>

      {/* User footer */}
      <div className="border-t px-2 py-3" style={{ borderColor: 'var(--sidebar-border)' }}>
        <div className="sidebar-item">
          <span className="flex h-7 w-7 shrink-0 items-center justify-center rounded-full bg-nebula-violet/20 text-xs font-bold text-nebula-violet">
            AD
          </span>
          <span
            className="overflow-hidden whitespace-nowrap transition-all duration-200"
            style={{ width: collapsed ? 0 : 'auto', opacity: collapsed ? 0 : 1 }}
          >
            <span className="text-sm font-medium" style={{ color: 'var(--text-primary)' }}>Admin</span>
          </span>
          {!collapsed && (
            <LogOut size={14} className="ml-auto shrink-0" style={{ color: 'var(--text-muted)' }} />
          )}
        </div>
      </div>
    </div>
  );

  return (
    <>
      {/* Desktop sidebar */}
      <div className="hidden lg:block">
        {sidebarContent}
      </div>

      {/* Mobile overlay */}
      {mobileOpen && (
        <div className="fixed inset-0 z-50 lg:hidden">
          {/* Backdrop */}
          <div
            className="absolute inset-0 bg-black/60 backdrop-blur-sm"
            onClick={closeMobile}
            aria-hidden="true"
          />
          {/* Sidebar panel */}
          <div className="relative w-64 h-full animate-slide-in-left">
            {/* Force expanded width in mobile */}
            <div className="sidebar h-full w-64 flex flex-col">
              {/* Header */}
              <div className="flex h-14 items-center justify-between px-3">
                <Link to="/" className="flex items-center gap-2">
                  <span className="flex h-8 w-8 shrink-0 items-center justify-center rounded-lg bg-nebula-violet/20 text-nebula-violet font-bold text-sm">
                    N
                  </span>
                  <span className="text-lg font-bold tracking-tight text-nebula-violet drop-shadow-[0_0_8px_rgba(139,92,246,0.3)]">
                    Nebula
                  </span>
                  <span className="text-sm" style={{ color: 'var(--text-muted)' }}>CRM</span>
                </Link>
                <button
                  onClick={closeMobile}
                  aria-label="Close navigation menu"
                  className="flex h-7 w-7 items-center justify-center rounded-md"
                  style={{ color: 'var(--text-muted)' }}
                >
                  <PanelLeftClose size={16} />
                </button>
              </div>

              {/* Notifications */}
              <div className="px-2 py-1">
                <NotificationDropdown
                  open={notifOpen}
                  onToggle={() => setNotifOpen((p) => !p)}
                  onClose={() => setNotifOpen(false)}
                  triggerRef={bellRef}
                />
              </div>

              {/* Navigation */}
              <div className="px-2 pt-4">
                <p className="mb-2 px-3 text-[10px] font-semibold uppercase tracking-widest"
                  style={{ color: 'var(--text-muted)' }}
                >
                  Navigation
                </p>
                <nav aria-label="Main navigation" className="space-y-1">
                  {NAV_ITEMS.map((item) => {
                    const active = isActive(item.href, pathname);
                    return (
                      <Link
                        key={item.href}
                        to={item.href}
                        aria-current={active ? 'page' : undefined}
                        className={cn('sidebar-item', active && 'sidebar-item-active')}
                      >
                        <item.icon size={20} className="shrink-0" />
                        <span>{item.label}</span>
                        {active && (
                          <ChevronRight size={14} className="ml-auto shrink-0 opacity-50" />
                        )}
                      </Link>
                    );
                  })}
                </nav>
              </div>

              <div className="flex-1" />

              {/* Theme toggle */}
              <div className="px-2 pb-1">
                <button onClick={toggleTheme} className="sidebar-item w-full"
                  aria-label={theme === 'dark' ? 'Switch to light mode' : 'Switch to dark mode'}
                >
                  {theme === 'dark' ? <Sun size={20} className="shrink-0" /> : <Moon size={20} className="shrink-0" />}
                  <span>{theme === 'dark' ? 'Light mode' : 'Dark mode'}</span>
                </button>
              </div>

              {/* User footer */}
              <div className="border-t px-2 py-3" style={{ borderColor: 'var(--sidebar-border)' }}>
                <div className="sidebar-item">
                  <span className="flex h-7 w-7 shrink-0 items-center justify-center rounded-full bg-nebula-violet/20 text-xs font-bold text-nebula-violet">
                    AD
                  </span>
                  <span className="text-sm font-medium" style={{ color: 'var(--text-primary)' }}>Admin</span>
                  <LogOut size={14} className="ml-auto shrink-0" style={{ color: 'var(--text-muted)' }} />
                </div>
              </div>
            </div>
          </div>
        </div>
      )}
    </>
  );
}
