import * as THREE from 'three';
import { GLTFLoader } from 'three/examples/jsm/loaders/GLTFLoader.js';

export class HumanComponent extends THREE.Group {
  constructor(modelPath: string) {
    super();

    const gltfLoader = new GLTFLoader();
    gltfLoader.load(modelPath, (gltfScene) => {
      gltfScene.scene.scale.set(13, 13, 13);

      gltfScene.scene.rotation.x = -Math.PI / 2;

      gltfScene.scene.traverse((child) => {
        if (child instanceof THREE.Mesh) {
          child.castShadow = true;
          child.receiveShadow = true;
        }
      });
      this.add(gltfScene.scene);
    });
  }
}
