import * as THREE from 'three';

export default class WallComponent extends THREE.Group {

  private frontMaterial: THREE.MeshPhongMaterial;

  constructor(
    frontTexturePath: string,
    rearTexturePath: string,
    frontColor: number = 0xa1ddfc,
    rearColor: number = 0xffffff
  ) {
    super();

    const textureLoader = new THREE.TextureLoader();
    const frontTexture = textureLoader.load(frontTexturePath);
    const rearTexture = textureLoader.load(rearTexturePath);

    [frontTexture, rearTexture].forEach((texture) => {
      texture.wrapS = THREE.RepeatWrapping;
      texture.wrapT = THREE.RepeatWrapping;
      texture.repeat.set(5, 1);
      texture.colorSpace = THREE.SRGBColorSpace;
      texture.magFilter = THREE.LinearFilter;
      texture.minFilter = THREE.LinearMipmapLinearFilter;
    });

    // Define materials with both textures and colors
    this.frontMaterial = new THREE.MeshPhongMaterial({
      color: frontColor,
      map: frontTexture,
      side: THREE.DoubleSide,
    });

    const rearMaterial = new THREE.MeshPhongMaterial({
      color: rearColor,
      map: rearTexture,
      side: THREE.DoubleSide,
    });

    // Create the front face
    const frontGeometry = new THREE.PlaneGeometry(0.95, 1.0);
    const frontFace = new THREE.Mesh(frontGeometry, this.frontMaterial);
    frontFace.position.set(0.0, 0.0, 0.025);
    frontFace.castShadow = true;
    frontFace.receiveShadow = true;
    this.add(frontFace);

    // Create the rear face with the unique rearMaterial
    const rearFace = new THREE.Mesh(frontGeometry, rearMaterial);
    rearFace.rotation.y = Math.PI;
    rearFace.position.set(0.0, 0.0, -0.025);
    rearFace.castShadow = true;
    rearFace.receiveShadow = true;
    this.add(rearFace);

    // Use the frontMaterial for all other faces (left, right, top, bottom)
    const sideGeometry = new THREE.PlaneGeometry(0.05, 1.0);

    // Left face
    const leftFace = new THREE.Mesh(sideGeometry, this.frontMaterial);
    leftFace.position.set(-0.475, 0.0, 0.0);
    leftFace.rotation.y = Math.PI / 2;
    leftFace.castShadow = true;
    leftFace.receiveShadow = true;
    this.add(leftFace);

    // Right face
    const rightFace = new THREE.Mesh(sideGeometry, this.frontMaterial);
    rightFace.position.set(0.475, 0.0, 0.0);
    rightFace.rotation.y = -Math.PI / 2;
    rightFace.castShadow = true;
    rightFace.receiveShadow = true;
    this.add(rightFace);

    // Top face
    const topGeometry = new THREE.PlaneGeometry(0.95, 0.05);
    const topFace = new THREE.Mesh(topGeometry, this.frontMaterial);
    topFace.position.set(0.0, 0.5, 0.0);
    topFace.rotation.x = Math.PI / 2;
    topFace.castShadow = true;
    topFace.receiveShadow = true;
    this.add(topFace);

    // Bottom face
    const bottomFace = new THREE.Mesh(topGeometry, this.frontMaterial);
    bottomFace.position.set(0.0, -0.5, 0.0);
    bottomFace.rotation.x = -Math.PI / 2;
    bottomFace.castShadow = true;
    bottomFace.receiveShadow = true;
    this.add(bottomFace);
  }

  public changeFrontColor(newColor: number): void {
    this.frontMaterial.color.set(newColor);
  }

}
