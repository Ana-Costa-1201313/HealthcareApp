import * as THREE from 'three';

export class DirectionalLightComponent extends THREE.Group {
  constructor() {
    super();

    const directionalLight = new THREE.DirectionalLight(0xffffff, 1);

    directionalLight.position.set(500, 500, 500);

    //shadow inactive
    directionalLight.castShadow = true;

    directionalLight.shadow.mapSize.width = 2048;
    directionalLight.shadow.mapSize.height = 2048;

    directionalLight.shadow.camera.near = 1;
    directionalLight.shadow.camera.far = 2000;
    directionalLight.shadow.camera.left = -500;
    directionalLight.shadow.camera.right = 500;
    directionalLight.shadow.camera.top = 500;
    directionalLight.shadow.camera.bottom = -500;

    this.add(directionalLight);
  }
}
