import type React from 'react'
import { render, screen } from '@testing-library/react'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import App from './App'

const authMocks = vi.hoisted(() => ({
  useAuthEventHandler: vi.fn(),
}))

vi.mock('./features/auth', () => ({
  useAuthEventHandler: authMocks.useAuthEventHandler,
  ProtectedRoute: ({ children }: { children: React.ReactNode }) => <>{children}</>,
}))

vi.mock('./pages/DashboardPage', () => ({
  default: () => <div>dashboard-page</div>,
}))

vi.mock('./pages/BrokerListPage', () => ({
  default: () => <div>broker-list-page</div>,
}))

vi.mock('./pages/CreateBrokerPage', () => ({
  default: () => <div>create-broker-page</div>,
}))

vi.mock('./pages/BrokerDetailPage', () => ({
  default: () => <div>broker-detail-page</div>,
}))

vi.mock('./pages/NotFoundPage', () => ({
  default: () => <div>not-found-page</div>,
}))

vi.mock('./pages/UnauthorizedPage', () => ({
  UnauthorizedPage: () => <div>unauthorized-page</div>,
}))

vi.mock('./pages/LoginPage', () => ({
  LoginPage: () => <div>login-page</div>,
}))

vi.mock('./pages/AuthCallbackPage', () => ({
  AuthCallbackPage: () => <div>auth-callback-page</div>,
}))

describe('App routing', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it.each([
    ['/', 'dashboard-page'],
    ['/brokers', 'broker-list-page'],
    ['/brokers/new', 'create-broker-page'],
    ['/brokers/broker-1', 'broker-detail-page'],
    ['/login', 'login-page'],
    ['/auth/callback', 'auth-callback-page'],
    ['/unauthorized', 'unauthorized-page'],
    ['/missing-route', 'not-found-page'],
  ])('renders %s at the correct route', (route, expectedText) => {
    window.history.replaceState({}, '', route)

    render(<App />)

    expect(screen.getByText(expectedText)).toBeInTheDocument()
    expect(authMocks.useAuthEventHandler).toHaveBeenCalledTimes(1)
  })
})
