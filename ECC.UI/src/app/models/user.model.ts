export interface User {
  userId: number;
  userName: string;
  userRoleCode: string; // Associated to User Role table (SM: Scrum Master, PO: Product Owner, DEV: Developer)
}
