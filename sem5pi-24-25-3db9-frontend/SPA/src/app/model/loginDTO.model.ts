export type LoginDTO = {
  result: any;
  value: {
    username: string | null;
    password: string | null;
    googleCredentials: string | null;
    jwt: string | null;
    role: string | null;
    tokenCreated: Date | null;
    a: number | null;
  }
}
