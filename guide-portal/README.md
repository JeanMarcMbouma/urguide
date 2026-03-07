# UrGuide Guide Portal

A React + TypeScript frontend for guides on the UrGuide tourism platform. Guides can manage their profiles, handle tour requests & bids, track earnings, and communicate with clients.

## Features

- **Profile Management** (Issue #172): Edit guide profile, specializations, languages, pricing, and KYC verification
- **Tour Requests & Bidding** (Issue #173): Browse tour requests, place/edit/withdraw bids, manage availability calendar
- **Earnings & Payouts** (Issue #174): Earnings dashboard with charts, transaction history, payout requests, and payment methods
- **Reviews & Communication** (Issue #175): View and respond to reviews, client messaging, and performance analytics

## Getting Started

```bash
npm install
npm run dev
```

The app runs on **port 3002** at [http://localhost:3002](http://localhost:3002).

## Environment Variables

Copy `.env.example` to `.env.local` and set values:

```
VITE_API_TARGET=http://localhost:5000
```

## Available Scripts

| Script | Description |
|--------|-------------|
| `npm run dev` | Start development server on port 3002 |
| `npm run build` | Build for production |
| `npm run preview` | Preview production build |
| `npm run lint` | Run ESLint |

## Tech Stack

- **React 18** with TypeScript
- **Material UI v6** for components
- **React Router v6** for routing
- **TanStack Query** for data fetching
- **Recharts** for charts
- **Vite** for build tooling
- **Axios** for HTTP requests
