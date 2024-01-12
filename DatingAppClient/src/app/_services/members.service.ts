import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { map, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];

  constructor(private http: HttpClient) { }

  getMembers() {
    if (this.members.length > 0)
      return of(this.members);

    return this.http.get<Member[]>(this.baseUrl + 'user/search/list').pipe(map(members => {
      this.members = members;
      return members;
    }));
  }

  getMemberById(id: number) {
    return this.http.get<Member>(this.baseUrl + 'user/search/id/' + id);
  }

  getMemberByUsername(username: string) {
    const member = this.members.find(x => x.userName === username);
    if (member)
      return of(member);

    return this.http.get<Member>(this.baseUrl + 'user/search/username/' + username);
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + "user", member).pipe(map(() => {
      const index = this.members.indexOf(member);
      this.members[index] = { ...this.members[index], ...member }
    }));
  }

  setProfilePicture(photoId: number) {
    return this.http.put(this.baseUrl + 'user/photo/update/profile-picture/' + photoId, {});
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'user/photo/delete/' + photoId, {});
  }
}
