import { Component, OnInit } from '@angular/core';
import { ProductService } from 'src/app/services/product.service';
import { Product } from 'src/app/models/product.model';
import { User } from 'src/app/models/user.model';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { DatePipe } from '@angular/common';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'app-edit-product',
  templateUrl: './edit-product.component.html',
  providers: [DatePipe],
  styleUrls: ['./edit-product.component.css']
})
export class EditProductComponent implements OnInit {
  updateProductRequest: Product = {
    productId: 0,
    productName: '',
    productNumber: 0,
    productOwner: {
      userId: 0,
      userName: '',
      userRoleCode: ''
    },
    scrumMaster: {
      userId: 0,
      userName: '',
      userRoleCode: ''
    },
    developers: [],
    location: '',
    methodology: '',
    startDate: new Date(),
    startDateFormatted: '',
  };
  developerList: Array<User> = [];
  selectedDeveloper: User = {
    userId: 0,
    userName: '',
    userRoleCode: 'DEV'
  };
  newDeveloper: User = {
    userId: 0,
    userName: '',
    userRoleCode: 'DEV'
  };
  showAlertBanner: boolean = false;
  alertBannerClass: string = '';
  alertMessage: string = '';

  constructor(private route: ActivatedRoute, private productService: ProductService,
    private http: HttpClient, private router: Router, private datePipe: DatePipe, private userService: UserService) {}



  ngOnInit(): void {
    // To grab id of the current product record
    this.route.paramMap.subscribe({
      next: (params) => {
        const id = params.get('id');

        if (id) {
          // Get details of the product
          this.productService.getProduct(id)
            .subscribe({
              next: (response) => {
                this.updateProductRequest = response;
                // Date format
                this.updateProductRequest.startDateFormatted = this.datePipe.transform(response.startDate, 'MMMM d, y') ?? 'Undefined';

              }
          });

          // Get all existing developers
          this.userService.getDevelopers()
            .subscribe({
              next: (response) => {
                this.developerList = response;
              }
          });

        }

      }
    });
  }

    // Update the product record when the form is submitted
    updateProduct() {
      if (this.updateProductRequest.startDateFormatted) {
        const parsedDate = this.datePipe.transform(this.updateProductRequest.startDateFormatted, 'MMMM d, y');
        if (parsedDate) {
          this.updateProductRequest.startDate = new Date(parsedDate);
        }
      }
      this.productService.updateProduct(this.updateProductRequest.productId.toString(), this.updateProductRequest)
        .subscribe({
          next: (response) => {
            this.router.navigate(['product/edit/' + this.updateProductRequest.productId]);
            // Set showSuccessBanner to true when the update is successful
            this.showAlertBanner = true;
            this.alertBannerClass = 'alert alert-success';
            this.alertMessage = 'Updated successfully';
          },
          error: (response) => {
            console.log(response.error);
            this.showAlertBanner = true;
            this.alertBannerClass = 'alert alert-danger';
            this.alertMessage = 'Update failed: ' + response.error;
          }
        });
    }

  // Add the selected developer to the developer list at the bottom
  addSelectedDeveloper(developer: User) {
    let developerRef = this.developerList.find(i => i.userName == developer.userName);
    if (developerRef != undefined)
    {
      // The developer is selected from the provided list. Add the developer to the product.
      this.updateProductRequest.developers?.push(developerRef);
      this.selectedDeveloper.userName = '';
    }
  }

  addNewDeveloper(developer: User) {
    if (developer.userName != '')
    {
      // The developer will be created as a new User and added to the product.
      let copiedDev: User = {
        userId: 0,
        userName: developer.userName,
        userRoleCode: 'DEV'
      };
      this.updateProductRequest.developers?.push(copiedDev);
      this.newDeveloper.userName = '';
    }
  }

  // Remove the developer from the list
  removeDeveloper(developer: User) {
    this.updateProductRequest.developers = this.updateProductRequest.developers?.filter(i => i.userId !== developer.userId);
  }

}
