const API_BASE = '/api'

interface ProblemDetails {
  type?: string
  title?: string
  status?: number
  detail?: string
  code?: string
  traceId?: string
  errors?: Record<string, string[]>
}

export class ApiError extends Error {
  constructor(
    public status: number,
    public problem: ProblemDetails | null,
  ) {
    super(problem?.title ?? `HTTP ${status}`)
    this.name = 'ApiError'
  }

  get code(): string | undefined {
    return this.problem?.code
  }

  get traceId(): string | undefined {
    return this.problem?.traceId
  }
}

async function fetchApi<T>(path: string, options?: RequestInit): Promise<T> {
  const response = await fetch(`${API_BASE}${path}`, {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...options?.headers,
    },
    credentials: 'include',
  })

  if (!response.ok) {
    const problem = await response.json().catch(() => null)
    throw new ApiError(response.status, problem)
  }

  return response.json()
}

export const api = {
  get: <T>(path: string) => fetchApi<T>(path),
  post: <T>(path: string, body: unknown) =>
    fetchApi<T>(path, { method: 'POST', body: JSON.stringify(body) }),
  put: <T>(path: string, body: unknown) =>
    fetchApi<T>(path, { method: 'PUT', body: JSON.stringify(body) }),
  delete: <T>(path: string) =>
    fetchApi<T>(path, { method: 'DELETE' }),
}
