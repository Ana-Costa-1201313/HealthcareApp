import * as THREE from 'three';
import { GLTFLoader } from 'three/examples/jsm/loaders/GLTFLoader.js';

export class LampComponent extends THREE.Group {
  private spotLight: THREE.SpotLight;
  private target: THREE.Object3D;
  private isLightOn: boolean = false;
  private audioClick: HTMLAudioElement;

  constructor(audioPath: string, modelPath: string) {
    super();

    this.audioClick = new Audio(audioPath);

    const gltfLoader = new GLTFLoader();
    gltfLoader.load(modelPath, (gltfScene) => {
      gltfScene.scene.scale.set(0.01, 0.01, 0.01);
      gltfScene.scene.position.set(6, 30.5, -2.5);

      gltfScene.scene.traverse((child) => {
        if (child instanceof THREE.Mesh) {
          child.castShadow = true;
          child.receiveShadow = true;
        }
      });
      this.add(gltfScene.scene);
    });

    this.spotLight = new THREE.SpotLight(
      0xffffff,
      500,
      500,
      Math.PI / 3,
      0.1,
      2
    );
    this.spotLight.position.set(6, 28, -2);
    this.spotLight.castShadow = true;

    this.spotLight.shadow.mapSize.width = 512;
    this.spotLight.shadow.mapSize.height = 512;
    this.spotLight.shadow.camera.near = 0.5;
    this.spotLight.shadow.camera.far = 50;

    this.target = new THREE.Object3D();
    this.target.position.set(6, 0, -2);
    this.spotLight.target = this.target;
    this.spotLight.intensity = 0;

    this.add(this.spotLight);
    this.add(this.target);
  }

  public toggleLight(): void {
    this.audioClick.currentTime = 0;
    this.audioClick.play();
    this.isLightOn = !this.isLightOn;
    this.spotLight.intensity = this.isLightOn ? 500 : 0;
  }
}
