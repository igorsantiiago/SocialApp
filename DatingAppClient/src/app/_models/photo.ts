export interface Photo {
    id: number;
    url: string;
    isProfile: boolean;
    isApproved: boolean;
    username?: string;
}