import { Component } from '@angular/core';
import * as THREE from 'three';

@Component({
  selector: 'app-spotlight',
  standalone: true,
  imports: [],
  templateUrl: './spotlight.component.html',
  styleUrls: ['./spotlight.component.scss'], // Fixed typo: styleUrls
})
export class SpotlightComponent extends THREE.Group {
  public target: THREE.Object3D;
  public spotlight: THREE.SpotLight;

  constructor() {
    super();

    this.target = new THREE.Object3D();
    this.target.position.set(0, 0, 0);

    this.spotlight = new THREE.SpotLight(0xffffff, 1000); 
    this.spotlight.position.set(20, 20, 20); 
    this.spotlight.castShadow = true;

    this.spotlight.shadow.camera.near = 1;
    this.spotlight.shadow.camera.far = 2000;
    // this.spotlight.shadow.mapSize.width = 1024; 
    this.spotlight.shadow.mapSize.height = 1024;

    this.spotlight.target = this.target;

    this.spotlight.angle = Math.PI/4;

    // const spotlightHelper = new THREE.SpotLightHelper(this.spotlight);
    // this.add(spotlightHelper);

    this.add(this.target);
    this.add(this.spotlight);
  }

  public animateSpotlight(
    startPosition: THREE.Vector3,
    endPosition: THREE.Vector3,
    startTarget: THREE.Vector3,
    endTarget: THREE.Vector3
  ) {
    const duration = 1; // Animation duration in seconds
    const fps = 60; // Frames per second
    const steps = duration * fps;
    let step = 0;
  
    const spotlight = this.spotlight;
  
    const animate = () => {
      if (step <= steps) {
        spotlight.position.lerpVectors(startPosition, endPosition, step / steps);
  
        const interpolatedTarget = new THREE.Vector3();
        interpolatedTarget.lerpVectors(startTarget, endTarget, step / steps);
        spotlight.target.position.copy(interpolatedTarget);
        spotlight.target.updateMatrixWorld();
  
        step++;
        requestAnimationFrame(animate); // Continue the animation
      }
    };
  
    animate();
  }
  
}
