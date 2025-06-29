import * as THREE from 'three';
import { GLTFLoader } from 'three/examples/jsm/loaders/GLTFLoader.js';

export class DoorComponent extends THREE.Group {
  public pivot: THREE.Group;
  public isOpen: boolean = false;
  private targetRotationY: number = 0;
  private rotationSpeed: number = 0.05;

  private audioOpen: HTMLAudioElement;
  private audioClose: HTMLAudioElement;

  constructor(
    audioOpenPath: string,
    audioClosePath: string,
    modelPath: string
  ) {
    super();

    this.pivot = new THREE.Group();
    this.add(this.pivot);

    const gltfLoader = new GLTFLoader();
    gltfLoader.load(modelPath, (gltfScene) => {
      gltfScene.scene.scale.setScalar(0.02);
      gltfScene.scene.position.set(0, 0, 0);
      gltfScene.scene.traverse((child) => {
        if (child instanceof THREE.Mesh) {
          child.castShadow = true;
          child.receiveShadow = true;
        }
      });
      this.pivot.add(gltfScene.scene);
      this.pivot.position.set(20, 0, 25);
    });

    this.audioOpen = new Audio(audioOpenPath);
    this.audioClose = new Audio(audioClosePath);

    this.animate();
  }

  public open() {
    if (!this.isOpen) {
      this.targetRotationY = Math.PI / 2;
      this.isOpen = true;
      this.audioOpen.currentTime = 0;
      this.audioOpen.play();
    }
  }

  public close() {
    if (this.isOpen) {
      this.targetRotationY = 0;
      this.isOpen = false;
      this.audioClose.currentTime = 0;
      this.audioClose.play();
    }
  }

  private animate() {
    requestAnimationFrame(() => this.animate());

    const deltaRotation = this.targetRotationY + this.pivot.rotation.y;

    if (Math.abs(deltaRotation) > 0.01) {
      this.pivot.rotation.y -= deltaRotation * this.rotationSpeed;
    }
  }
}
