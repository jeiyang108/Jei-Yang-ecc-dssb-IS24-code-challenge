import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { Product } from '../models/product.model';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  baseApiUrl: string = environment.baseApiUrl;

  constructor(private http: HttpClient) { }

  getAllProducts(): Observable<Product[]> {
    return this.http.get<Product[]>(this.baseApiUrl + '/api/product')
  }

  getProduct(id: string): Observable<Product> {
    return this.http.get<Product>(this.baseApiUrl + '/api/product/' + id);
  }

  updateProduct(id: string, updateProductRequest: Product): Observable<Product> {
    return this.http.put<Product>(this.baseApiUrl + '/api/product/' + id, updateProductRequest);
  }

  createProduct(addProductRequest: Product): Observable<Product> {
    return this.http.post<Product>(this.baseApiUrl + '/api/product', addProductRequest);
  }
}
