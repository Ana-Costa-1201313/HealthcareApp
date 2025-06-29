import { Result } from "../logic/Result";

export interface Repo<T> {
  exists (t: T): Promise<Result<boolean>>;
  save (t: T): Promise<Result<T>>;
}