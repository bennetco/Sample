import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { HelloWorld } from './helloworld'
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class HelloWorldService {
  private apiUrlBasic = 'http://localhost:56503/api/GetHelloWorldInfo';
  private apiUrlMany = 'http://localhost:56503/api/GetManyHelloWorldInfoByQuantity';

  constructor(private http: HttpClient) { }

  getHelloWorld(): Observable<HelloWorld> {
    let result = this.http.get<HelloWorld>(this.apiUrlBasic);
    return result;
  }

  getManyHelloWorld(quantity : number): Observable<HelloWorld[]> {
    let result = this.http.get<HelloWorld[]>(this.apiUrlMany + "?quantity=" + quantity);
    return result;
  }
}
