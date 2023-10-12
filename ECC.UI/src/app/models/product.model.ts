import { User } from "./user.model";

export interface Product {
  productId: number;
  productNumber: number;
  productName: string;
  productOwner: User;
  developers: Array<User>;
  developerNames?: string;
  scrumMaster: User;
  startDate: Date;
  methodology: string;
  location: string;
  startDateFormatted?: string;
}
