import * as THREE from 'three';

export class SpriteComponent extends THREE.Group {
  constructor(text: string) {
    super();

    const texture = this.createTextTexture(text);
    const spriteMaterial = new THREE.SpriteMaterial({
      map: texture,
      transparent: true,
    });
    const sprite = new THREE.Sprite(spriteMaterial);
    sprite.scale.set(texture.image.width / 10, texture.image.height / 10, 1);
    sprite.position.set(8, 45, -30);

    this.add(sprite);
  }

  private createTextTexture(text: string): THREE.Texture {
    const canvas = document.createElement('canvas');
    const context = canvas.getContext('2d')!;

    const fontSize = 30;
    canvas.width = text.length * fontSize;
    canvas.height = fontSize * 2;

    context.font = `${fontSize}px Arial`;
    context.fillStyle = 'black';
    context.textAlign = 'center';
    context.textBaseline = 'middle';
    context.fillText(text, canvas.width / 2, canvas.height / 2);

    const texture = new THREE.CanvasTexture(canvas);
    texture.needsUpdate = true;

    return texture;
  }
}
