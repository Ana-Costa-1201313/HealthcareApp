import * as THREE from 'three';
import * as TWEEN from '@tweenjs/tween.js';
import { GLTFLoader } from 'three/examples/jsm/loaders/GLTFLoader.js';
import { HumanComponent } from '../human/human.component';
import { OrbitControls } from 'three/examples/jsm/controls/OrbitControls.js';

export class TableComponent extends THREE.Group {
  private human: HumanComponent | null = null;

  constructor(modelPath: string, humanModelPath: string, cirurgy: boolean) {
    super();

    const gltfLoader = new GLTFLoader();

    gltfLoader.load(modelPath, (gltfScene) => {
      gltfScene.scene.scale.set(10, 10, 10);
      gltfScene.scene.position.set(0, 0, 0);
      gltfScene.scene.traverse((child) => {
        if (child instanceof THREE.Mesh) {
          child.castShadow = true;
          child.receiveShadow = true;
        }
      });
      this.add(gltfScene.scene);

      this.human = new HumanComponent(humanModelPath);
      this.human.visible = cirurgy;
      this.human.position.set(-2, 20, -6);
      this.add(this.human);
    });
  }

  public pointCameraTowards(controls: OrbitControls, camera: THREE.PerspectiveCamera): void {
    const tablePosition = this.getWorldPosition(new THREE.Vector3());
    const startPosition = camera.position.clone();
    const endPosition = new THREE.Vector3(tablePosition.x - 30, tablePosition.y + 30, tablePosition.z + 5);
    const startTarget = controls.target.clone();
    const endTarget = new THREE.Vector3(tablePosition.x + 5, tablePosition.y + 13, tablePosition.z - 10);

    const duration = 1000; // Duration of the animation in milliseconds
    let startTime: number | null = null;

    const animate = (time: number) => {
      if (startTime === null) startTime = time;
      const elapsed = time - startTime;
      const t = Math.min(elapsed / duration, 1); // Normalized time (0 to 1)

      // Interpolate camera position
      camera.position.lerpVectors(startPosition, endPosition, t);
      // Interpolate controls target
      controls.target.lerpVectors(startTarget, endTarget, t);
      controls.update();

      if (t < 1) {
        requestAnimationFrame(animate);
      }
    };

    requestAnimationFrame(animate);
  }
}
