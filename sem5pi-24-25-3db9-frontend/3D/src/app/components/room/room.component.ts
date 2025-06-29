import * as THREE from 'three';
import { DoorComponent } from '../door/door.component';
import FloorComponent from '../floor/floor.component';
import InteriorWallComponent from '../interior-wall/interior-wall.component';
import { LampComponent } from '../lamp/lamp.component';
import { TableComponent } from '../table/table.component';
import WallComponent from '../wall/wall.component';
import { OrbitControls } from 'three/examples/jsm/controls/OrbitControls.js';
import { AppointmentDates } from '../../model/appointmentDates.model';
import { AppComponent } from '../../app.component';
import { Appointment } from '../../model/appointment.model';

export default class RoomComponent extends THREE.Group {
  public roomName: string;
  private door: DoorComponent;
  private lamp: LampComponent;
  public table: TableComponent;
  private wall: InteriorWallComponent | undefined;
  private wall1: InteriorWallComponent | undefined;
  private wall2: InteriorWallComponent | undefined;
  private wall3: InteriorWallComponent | undefined;
  private wall4: InteriorWallComponent | undefined;
  private wall5: WallComponent | undefined;
  public appointmentsDates: AppointmentDates[] = [];
  public currentAppointment: Appointment | null;

  constructor(
    floorTexturePath: string,
    floorWidth: number,
    floorDepth: number,
    tableModelPath: string,
    humanModelPath: string,
    doorAudioOpenPath: string,
    doorAudioClosePath: string,
    doorModelPath: string,
    lampAudioPath: string,
    lampModelPath: string,
    wallFrontTexturePath: string,
    wallRearTexturePath: string,
    wallFrontColor: number,
    wallRearColor: number,
    woodPanelFrontTexturePath: string,
    woodPaneRearTexturePath: string,
    woodPanelFrontColor: number,
    woodPanelRearColor: number,
    currentAppointment: Appointment | null,
    roomName: string,
    appointmentsDates: AppointmentDates[]
  ) {
    super();

    this.roomName = roomName;
    this.appointmentsDates = appointmentsDates || [];
    this.currentAppointment = currentAppointment;


    const floor = new FloorComponent(floorTexturePath, floorWidth, floorDepth);
    floor.position.set(7, 0.01, -3);
    this.add(floor);

    let cirurgy;
    if (currentAppointment) {
      cirurgy = true;
    } else {
      cirurgy = false;
    }

    this.table = new TableComponent(tableModelPath, humanModelPath, cirurgy);
    this.table.position.set(9, -10.6, -3);
    this.add(this.table);

    this.door = new DoorComponent(
      doorAudioOpenPath,
      doorAudioClosePath,
      doorModelPath
    );
    this.add(this.door);

    this.lamp = new LampComponent(lampAudioPath, lampModelPath);
    this.add(this.lamp);

    this.addWalls(
      wallFrontTexturePath,
      wallRearTexturePath,
      wallFrontColor,
      wallRearColor,
      woodPanelFrontTexturePath,
      woodPaneRearTexturePath,
      woodPanelFrontColor,
      woodPanelRearColor
    );
  }

  public onRoomClick(event: MouseEvent, camera: THREE.PerspectiveCamera, controls: OrbitControls) {
    const mouse = new THREE.Vector2(
      (event.clientX / window.innerWidth) * 2 - 1,
      -(event.clientY / window.innerHeight) * 2 + 1
    );

    const raycaster = new THREE.Raycaster();
    raycaster.setFromCamera(mouse, camera);

    const doorIntersects = raycaster.intersectObject(this.door.pivot, true);
    if (doorIntersects.length > 0) {
      this.door.isOpen ? this.door.close() : this.door.open();
      this.pickRoom()
      return true;
    }

    const lampIntersects = raycaster.intersectObjects(this.lamp.children, true);
    if (lampIntersects.length > 0) {
      this.lamp.toggleLight();
      this.pickRoom()
      return true;
    }

    const tableIntersects = raycaster.intersectObjects(this.table.children, true);
    if (tableIntersects.length > 0) {
      this.table.pointCameraTowards(controls, camera);
      this.pickRoom()
      return true;
    }

    const roomIntersects = raycaster.intersectObjects(this.children, true);
    if (roomIntersects.length > 0) {
      this.pickRoom();
      return true;
    }

    return false;
  }

  public getTablePosition() {
    var tablePosition = this.table.position.clone();
    return this.getWorldPosition(tablePosition);
  }

  private addWalls(
    wallFrontTexturePath: string,
    wallRearTexturePath: string,
    wallFrontColor: number,
    wallRearColor: number,
    woodPanelFrontTexturePath: string,
    woodPaneRearTexturePath: string,
    woodPanelFrontColor: number,
    woodPanelRearColor: number
  ) {
    this.wall = new InteriorWallComponent(
      wallFrontTexturePath,
      wallRearTexturePath,
      wallFrontColor,
      wallRearColor,
      woodPanelFrontTexturePath,
      woodPaneRearTexturePath,
      woodPanelFrontColor,
      woodPanelRearColor
    );
    this.wall.position.set(29.5, 15, 25);
    this.wall.scale.set(20, 30, 20);
    this.add(this.wall);

    this.wall1 = new InteriorWallComponent(
      wallFrontTexturePath,
      wallRearTexturePath,
      wallFrontColor,
      wallRearColor,
      woodPanelFrontTexturePath,
      woodPaneRearTexturePath,
      woodPanelFrontColor,
      woodPanelRearColor
    );
    this.wall1.rotation.y = Math.PI / 2; // Rotate 90 degrees
    this.wall1.position.set(38.6, 15, -3.2);
    this.wall1.scale.set(60, 30, 20);
    this.add(this.wall1);

    this.wall2 = new InteriorWallComponent(
      wallFrontTexturePath,
      wallRearTexturePath,
      wallFrontColor,
      wallRearColor,
      woodPanelFrontTexturePath,
      woodPaneRearTexturePath,
      woodPanelFrontColor,
      woodPanelRearColor
    );
    this.wall2.rotation.y = -Math.PI / 2; // Rotate 90 degrees
    this.wall2.position.set(-25, 15, -3.2);
    this.wall2.scale.set(60, 30, 20);
    this.add(this.wall2);

    this.wall3 = new InteriorWallComponent(
      wallFrontTexturePath,
      wallRearTexturePath,
      wallFrontColor,
      wallRearColor,
      woodPanelFrontTexturePath,
      woodPaneRearTexturePath,
      woodPanelFrontColor,
      woodPanelRearColor
    );
    this.wall3.rotation.y = -Math.PI; // Rotate 180 degrees
    this.wall3.position.set(6.8, 15, -31.5);
    this.wall3.scale.set(67.5, 30, 20);
    this.add(this.wall3);

    this.wall4 = new InteriorWallComponent(
      wallFrontTexturePath,
      wallRearTexturePath,
      wallFrontColor,
      wallRearColor,
      woodPanelFrontTexturePath,
      woodPaneRearTexturePath,
      woodPanelFrontColor,
      woodPanelRearColor
    );
    this.wall4.position.set(-9.7, 15, 25);
    this.wall4.scale.set(33, 30, 20);
    this.add(this.wall4);

    this.wall5 = new WallComponent(
      wallFrontTexturePath,
      wallRearTexturePath,
      wallFrontColor,
      wallRearColor
    );
    this.wall5.position.set(13, 27.27, 25);
    this.wall5.scale.set(15, 5.45, 20);
    this.wall5.rotation.z = -Math.PI; // Rotate 90 degrees
    this.add(this.wall5);
  }

  public pickRoom(): void {
    this.wall1?.getWall().changeFrontColor(0x2dc3ed);
    this.wall2?.getWall().changeFrontColor(0x2dc3ed);
    this.wall3?.getWall().changeFrontColor(0x2dc3ed);
    this.wall4?.getWall().changeFrontColor(0x2dc3ed);
    this.wall?.getWall().changeFrontColor(0x2dc3ed);
    this.wall5?.changeFrontColor(0x2dc3ed);
  }

  public unpickRoom(): void {
    this.wall1?.getWall().changeFrontColor(0xa1ddfc);
    this.wall2?.getWall().changeFrontColor(0xa1ddfc);
    this.wall3?.getWall().changeFrontColor(0xa1ddfc);
    this.wall4?.getWall().changeFrontColor(0xa1ddfc);
    this.wall?.getWall().changeFrontColor(0xa1ddfc);
    this.wall5?.changeFrontColor(0xa1ddfc);
  }


}
