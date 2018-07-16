import { Component, OnInit } from '@angular/core';
import { HelloWorldService } from '../helloworld.service'
import { HelloWorld } from '../helloworld';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-helloworld',
  templateUrl: './helloworld.component.html'
})
export class HelloWorldComponent implements OnInit {

  helloWorldContent$: Observable<HelloWorld[]>;

  constructor(private helloWorld: HelloWorldService) { }

  ngOnInit() {
    this.helloWorldContent$ = this.helloWorld.getManyHelloWorld(10);
  }

}
