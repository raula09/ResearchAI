export function saveToken(t: string) { localStorage.setItem("rc_token", t) }
export function loadToken(): string | null { return localStorage.getItem("rc_token") }
export function clearToken() { localStorage.removeItem("rc_token") }
