import { Db } from "mongodb";
import mongoose from "mongoose";
import config from "../../config";

mongoose.set('strictQuery', false);

export default async (): Promise<Db> => {
  const connection = await mongoose.connect(config.databaseURL);
  return connection.connection.db;
};
