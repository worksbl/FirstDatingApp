import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { analyzeAndValidateNgModules } from '@angular/compiler';

@Component({
  selector: 'app-value',
  templateUrl: './value.component.html',
  styleUrls: ['./value.component.css']
})
export class ValueComponent implements OnInit {
  values: any;

  constructor(private http: HttpClient) { } /*Used to make http request to Client Server*/

  ngOnInit() {

    this.getValues();
  }

  getValues(){
    this.http.get("http://localhost:5000/Api/Values/")
      .subscribe(response => this.values = response,
      error => {console.log(error);
      });
  }

}
