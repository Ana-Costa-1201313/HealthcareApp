import * as THREE from 'three';

export class AmbientLightComponent extends THREE.Group {
  constructor() {
    super();

    const ambientLight = new THREE.AmbientLight(0xffffff, 1);
    this.add(ambientLight);
  }
}
