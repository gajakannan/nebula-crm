/**
 * Dev-only: auto-fetches a Keycloak token using password grant.
 * Remove this when real OIDC login flow is implemented.
 */

const KEYCLOAK_URL = 'http://localhost:8081/realms/nebula/protocol/openid-connect/token';
const CLIENT_ID = 'nebula-crm';
const USERNAME = 'admin';
const PASSWORD = 'password';

let cachedToken: string | null = null;
let tokenExpiry = 0;

export async function getDevToken(): Promise<string> {
  // Return cached token if still valid (with 30s buffer)
  if (cachedToken && Date.now() < tokenExpiry - 30_000) {
    return cachedToken;
  }

  const response = await fetch(KEYCLOAK_URL, {
    method: 'POST',
    headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
    body: new URLSearchParams({
      grant_type: 'password',
      client_id: CLIENT_ID,
      username: USERNAME,
      password: PASSWORD,
    }),
  });

  if (!response.ok) {
    throw new Error(`Dev auth failed: ${response.status}`);
  }

  const data = await response.json();
  cachedToken = data.access_token;
  tokenExpiry = Date.now() + data.expires_in * 1000;
  return cachedToken!;
}
