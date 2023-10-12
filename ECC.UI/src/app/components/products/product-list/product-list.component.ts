import { ProductService } from './../../../services/product.service';
import { Component, OnInit, ViewChild } from '@angular/core';
import { Product } from 'src/app/models/product.model';
import { User } from 'src/app/models/user.model';
import { HttpClient } from '@angular/common/http';
import { FormControl } from '@angular/forms';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.css']
})


export class ProductListComponent implements OnInit {

  users: User[] = [];
  products: Product[] = [];
  columns: string[] = ['productNumber', 'productName', 'scrumMaster', 'productOwner', 'developerNames', 'startDate', 'methodology', 'location'];
  dataSource!: MatTableDataSource<Product>;

  // MatPaginator Output
  pageEvent!: PageEvent;
  /* ViewChild: a local component template*/
  @ViewChild('paginator') paginator!: MatPaginator;

  constructor(private productService: ProductService, private http: HttpClient) {}

  ngOnInit(): void {
    // Get list of products

    this.productService.getAllProducts()
    .subscribe({
      next: (products) => {
        this.products = products;
        this.products.forEach(product => {
          product.developerNames = product.developers.map(dev => dev.userName).join(', ');
        });
        // Angular Material (Paginator table)
        this.dataSource = new MatTableDataSource<Product>(products);
        this.dataSource.paginator = this.paginator;
        this.dataSource.filteredData = this.dataSource.data.slice(0, products.length < 5 ? products.length : this.dataSource.paginator?.pageSize);
      },
      error: (response) => {
        console.log(response);
      }
    });

   /*
    this.products.forEach(product => {
      product.developerNames = product.developers.map(dev => dev.userName).join(', ');
    });
    this.dataSource = new MatTableDataSource<Product>(this.products);
    this.dataSource.paginator = this.paginator;
    this.dataSource.filteredData = this.dataSource.data.slice(0, this.pageSize);
    */
  }

  //------- Search functionality -------
  // Search options
  searchOptions = ['Scrum Master', 'Developer', 'Any'];
  // Selected search type
  selectedSearchType = 'Any';
  searchText = '';

  // Filter the products based on search criteria
  filterProducts(event: any) {
    if (this.searchText == '') {
      this.dataSource.data =  this.products;
    } else if (this.selectedSearchType === 'Any') {
      // Search by any field
      this.dataSource.data =  this.products.filter(product => {
        return (
          product.productName.toLowerCase().includes(this.searchText.toLowerCase()) ||
          product.productOwner.userName.toLowerCase().includes(this.searchText.toLowerCase()) ||
          product.scrumMaster.userName.toLowerCase().includes(this.searchText.toLowerCase()) ||
          product.developers.some(dev => dev.userName.toLowerCase().includes(this.searchText.toLowerCase()))
        );
      });
    } else if (this.selectedSearchType === 'Scrum Master') {
      // Search by Scrum Master's name
      this.dataSource.data =  this.products.filter(product => {
        return product.scrumMaster.userName.toLowerCase().includes(this.searchText.toLowerCase());
      });
    } else if (this.selectedSearchType === 'Developer') {
      // Search by Developer's name
      this.dataSource.data = this.products.filter(product => {
        return product.developers.some(dev => dev.userName.toLowerCase().includes(this.searchText.toLowerCase()));
      });
    }

    // Pagination
    this.dataSource.filteredData = this.dataSource.data.slice(0, this.dataSource.data.length < 5 ? this.dataSource.data.length : this.dataSource.paginator?.pageSize);
  }

  //----------- Pagination -----------
  onPageChanged(event: any) {
    let firstCut = event.pageIndex * event.pageSize;
    let secondCut = firstCut + event.pageSize;
    this.dataSource.filteredData = this.dataSource.data.slice(firstCut, secondCut);
  }
}


// Function to generate a unique ID
function generateUniqueId() {
  return '_' + Math.random().toString(36).substr(2, 9);
}
