import { http, HttpResponse } from 'msw';

const API_BASE = process.env.EXPO_PUBLIC_API_URL || 'http://localhost:3000';

/**
 * Order API mock handlers
 */
export const orderHandlers = [
  // POST /api/orders
  http.post(`${API_BASE}/api/orders`, async ({ request }) => {
    const body = await request.json() as any;

    if (!body.items || body.items.length === 0) {
      return HttpResponse.json(
        { error: 'Order must contain at least one item' },
        { status: 400 }
      );
    }

    const order = {
      id: 'order-' + Math.random().toString(36).substr(2, 9),
      items: body.items,
      total: body.total || 0,
      status: 'pending',
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    };

    return HttpResponse.json(order, { status: 201 });
  }),

  // GET /api/orders
  http.get(`${API_BASE}/api/orders`, () => {
    const orders = [
      {
        id: 'order-1',
        items: [{ id: 'item-1', quantity: 2 }],
        total: 25.98,
        status: 'completed',
        createdAt: new Date(Date.now() - 86400000).toISOString(),
      },
      {
        id: 'order-2',
        items: [{ id: 'item-2', quantity: 1 }],
        total: 12.99,
        status: 'pending',
        createdAt: new Date().toISOString(),
      },
    ];

    return HttpResponse.json(
      {
        data: orders,
        total: orders.length,
      },
      { status: 200 }
    );
  }),

  // GET /api/orders/:orderId
  http.get(`${API_BASE}/api/orders/:orderId`, ({ params }) => {
    const { orderId } = params;

    if (!orderId || orderId === 'invalid') {
      return HttpResponse.json(
        { error: 'Order not found' },
        { status: 404 }
      );
    }

    const order = {
      id: orderId,
      items: [{ id: 'item-1', quantity: 2 }],
      total: 25.98,
      status: 'completed',
      createdAt: new Date().toISOString(),
    };

    return HttpResponse.json(order, { status: 200 });
  }),

  // POST /api/orders/:orderId/cancel
  http.post(`${API_BASE}/api/orders/:orderId/cancel`, ({ params }) => {
    const { orderId } = params;

    if (!orderId || orderId === 'invalid') {
      return HttpResponse.json(
        { error: 'Order not found' },
        { status: 404 }
      );
    }

    return HttpResponse.json(
      { success: true, status: 'cancelled' },
      { status: 200 }
    );
  }),
];
