import { User } from "./user";

export class UserParams {
    gender: string;
    minimumAge = 18;
    maximumAge = 99;
    pageNumber = 1;
    pageSize = 12;
    orderBy = 'lastActivity';

    constructor(user: User) {
        this.gender = user.gender === 'masculino' ? 'feminino' : 'masculino';
    }
}