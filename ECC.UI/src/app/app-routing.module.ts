import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProductListComponent } from './components/products/product-list/product-list.component';
import { EditProductComponent } from './components/products/edit-product/edit-product.component';
import { AddProductComponent } from './components/products/add-product/add-product.component';

const routes: Routes = [
  {
    path: '',
    component: ProductListComponent
  },
  {
    path: 'products',
    component: ProductListComponent
  },
  {
    path: 'product/edit/:id',
    component: EditProductComponent
  },
  {
    path: 'product/add',
    component: AddProductComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
