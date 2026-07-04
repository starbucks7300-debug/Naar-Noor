import { http, HttpResponse } from 'msw';
import { createMockUser, createMockTokenPair } from '../factories/userFactory';

const API_BASE = process.env.EXPO_PUBLIC_API_URL || 'http://localhost:3000';

/**
 * Auth API mock handlers
 */
export const authHandlers = [
  // POST /api/auth/register
  http.post(`${API_BASE}/api/auth/register`, async ({ request }) => {
    const body = await request.json() as any;

    if (!body.email || !body.password) {
      return HttpResponse.json(
        { error: 'Email and password are required' },
        { status: 400 }
      );
    }

    const user = createMockUser({ email: body.email });
    const tokenPair = createMockTokenPair();

    return HttpResponse.json(
      {
        user,
        ...tokenPair,
      },
      { status: 201 }
    );
  }),

  // POST /api/auth/login
  http.post(`${API_BASE}/api/auth/login`, async ({ request }) => {
    const body = await request.json() as any;

    if (!body.email || !body.password) {
      return HttpResponse.json(
        { error: 'Email and password are required' },
        { status: 400 }
      );
    }

    if (body.password !== 'password') {
      return HttpResponse.json(
        { error: 'Invalid email or password' },
        { status: 401 }
      );
    }

    const user = createMockUser({ email: body.email });
    const tokenPair = createMockTokenPair();

    return HttpResponse.json(
      {
        user,
        ...tokenPair,
      },
      { status: 200 }
    );
  }),

  // POST /api/auth/refresh
  http.post(`${API_BASE}/api/auth/refresh`, () => {
    const tokenPair = createMockTokenPair();

    return HttpResponse.json(tokenPair, { status: 200 });
  }),

  // POST /api/auth/logout
  http.post(`${API_BASE}/api/auth/logout`, () => {
    return HttpResponse.json({ success: true }, { status: 200 });
  }),

  // GET /api/auth/me
  http.get(`${API_BASE}/api/auth/me`, () => {
    const user = createMockUser();

    return HttpResponse.json(user, { status: 200 });
  }),
];
