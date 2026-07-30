function required(name: string): string {
  const value = process.env[name];
  if (!value) {
    throw new Error(
      `Missing required environment variable ${name} - see apps/web/.env.example`,
    );
  }
  return value;
}

export const env = {
  API_BASE_URL: required("API_BASE_URL"),
} as const;
