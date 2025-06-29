import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import * as THREE from 'three';
import { OrbitControls } from 'three/examples/jsm/controls/OrbitControls.js';
import { Appointment } from '../../model/appointment.model';
import { AppointmentService } from '../../service/appointment.service';
import {
  doorData,
  hospitalFloorData,
  humanData,
  lampData,
  roomFloorData,
  tableData,
  wallData,
  wallWoodPanelData,
} from '../defaul-data/defaul-data.component';
import FloorComponent from '../floor/floor.component';
import RoomComponent from '../room/room.component';
import { SpriteComponent } from '../sprite/sprite.component';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AmbientLightComponent } from '../ambient-light/ambient-light.component';
import { DirectionalLightComponent } from '../directional-light/directional-light.component';
import { OrbitControlsComponent } from '../orbit-controls/orbit-controls.component';
import { SpotlightComponent } from '../spotlight/spotlight.component';
import { AppointmentDates } from '../../model/appointmentDates.model';
import { parse, isWithinInterval } from 'date-fns';
import { AppComponent } from '../../app.component';


@Component({
  selector: 'app-hospital',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './hospital.component.html',
  styleUrl: './hospital.component.css',
})
export class HospitalComponent implements OnInit {
  @ViewChild('canvas') private canvasRef!: ElementRef;
  private rooms: RoomComponent[] = [];
  apList: Appointment[] = [];

  private renderer!: THREE.WebGLRenderer;
  private scene: THREE.Scene = new THREE.Scene();
  private camera!: THREE.PerspectiveCamera;
  private camera2!: THREE.PerspectiveCamera;
  private controls1!: OrbitControlsComponent;
  private controls2!: OrbitControlsComponent;
  private activeControls!: OrbitControlsComponent;
  public currentPickedRoom: RoomComponent | null | undefined = undefined;
  private spotlight = new SpotlightComponent();
  private spotlightHelper = new THREE.SpotLightHelper(this.spotlight.spotlight);

  appointmentsVisible: boolean = false;
  currentRoomName: string = '';
  isInteractionDisabled = false;
  filteredAppointments: Appointment[] = [];

  roomsJson: any;

  // Stage properties
  @Input() public cameraZ: number = 20;
  @Input() public fieldOfView: number = 30;
  @Input('nearClipping') public nearClippingPlane: number = 1;
  @Input('farClipping') public farClippingPlane: number = 10000;

  private toggleControl(): void {
    if (this.activeControls === this.controls1) {
      this.controls1.controls.enabled = false;
      this.controls2.controls.enabled = true;
      this.activeControls = this.controls2;
    } else {
      this.controls2.controls.enabled = false;
      this.controls1.controls.enabled = true;
      this.activeControls = this.controls1;
    }
  }

  private setupControls(): void {
    // Controls for the cameras
    this.controls1 = new OrbitControlsComponent(this.camera, this.renderer);
    this.controls2 = new OrbitControlsComponent(this.camera2, this.renderer);

    // Default active camera and controls
    this.controls1.controls.enabled = true;
    this.controls2.controls.enabled = false;

    this.activeControls = this.controls1;
  }

  //helper
  private get canvas(): HTMLCanvasElement {
    return this.canvasRef.nativeElement;
  }

  private getAspectRatio(): number {
    return this.canvas.clientWidth / this.canvas.clientHeight;
  }

  private createScene(): void {
    this.scene.background = new THREE.Color(0x89cff0);

    var size = 0;

    if (this.roomsJson && this.roomsJson.rooms) {
      this.roomsJson.rooms.forEach((roomData: any, index: number) => {

        this.setupRoom(index, roomData);

        size++;

        this.setupFloor(index);
      });

      this.updateCameraWithDelay();
    }

    this.setupLights();
  }

  private updateCameraWithDelay() {
    setTimeout(() => {
      this.updateCamera2Position();
    }, 100);
  }

  private dateNow(): string {
    const now = new Date();
    const day = now.getDate().toString().padStart(2, '0');
    const month = (now.getMonth() + 1).toString().padStart(2, '0');
    const year = now.getFullYear();
    const hours = now.getHours().toString().padStart(2, '0');
    const minutes = now.getMinutes().toString().padStart(2, '0');
    const seconds = now.getSeconds().toString().padStart(2, '0');
    return `${day}/${month}/${year} ${hours}:${minutes}:${seconds}`;
  }


  private isRoomCurrentlyOccupied(
    appointmentsDates: AppointmentDates[],
    currentDateTime: string,
    appList: Appointment[]
  ): Appointment | null {
    var currentAp = null;
    const currentDate = parse(currentDateTime, 'dd/MM/yyyy HH:mm:ss', new Date());

    if (isNaN(currentDate.getTime())) {
      console.error("Invalid currentDateTime format:", currentDateTime);
      return currentAp;
    }

    for (const appointment of appointmentsDates) {
      if (!appointment.startDateTime || !appointment.endDateTime) {
        console.warn("Missing start or end date in appointment:", appointment);
        continue;
      }

      const start = parse(appointment.startDateTime, 'dd/MM/yyyy HH:mm:ss', new Date());
      const end = parse(appointment.endDateTime, 'dd/MM/yyyy HH:mm:ss', new Date());

      if (isNaN(start.getTime()) || isNaN(end.getTime())) {
        console.warn("Invalid date format detected in appointment:", appointment);
        continue;
      }

      if (isWithinInterval(currentDate, { start, end })) {
        currentAp = appList.find((app) => String(app.dateTime) === String(appointment.startDateTime));
        return currentAp || null;
      }
    }

    return currentAp;
  }

  private setupRoom(index: number, roomData: any) {

    const appointmentDates = this.getAppointmentsDatesForRoom(roomData.name);

    const currentDateTime = this.dateNow();

    const isRoomOccupied = this.isRoomCurrentlyOccupied(appointmentDates, currentDateTime, this.apList);

    const room = new RoomComponent(
      roomFloorData.texturePath,
      roomFloorData.floorWidth,
      roomFloorData.floorDepth,
      tableData.modelPath,
      humanData.modelPath,
      doorData.audioOpenPath,
      doorData.audioClosePath,
      doorData.modelPath,
      lampData.audioPath,
      lampData.modelPath,
      wallData.frontTexturePath,
      wallData.rearTexturePath,
      wallData.frontColor,
      wallData.rearColor,
      wallWoodPanelData.frontTexturePath,
      wallWoodPanelData.rearTexturePath,
      wallWoodPanelData.frontColor,
      wallWoodPanelData.rearColor,
      isRoomOccupied,
      roomData.name,
      appointmentDates
    );

    room.position.set(index * 85, 0, 0);

    this.setupSprite(room, index);

    this.scene.add(room);
    this.rooms.push(room);
  }

  private setupSprite(room: any, index: number) {
    const sprite = new SpriteComponent(room.roomName);
    sprite.position.set(index * 85, 0, 0);
    this.scene.add(sprite);
  }

  private setupFloor(index: number) {
    const floor3 = new FloorComponent(
      hospitalFloorData.texturePath,
      hospitalFloorData.floorWidth,
      hospitalFloorData.floorDepth
    );
    floor3.position.set(7 + 85 * index, -0.01, 0);
    this.scene.add(floor3);
  }

  private setupLights() {
    const ambientLight = new AmbientLightComponent();
    const directionalLight = new DirectionalLightComponent();

    // // this.spotlight = new SpotlightComponent();
    // this.spotlightHelper = new THREE.SpotLightHelper(this.spotlight.spotlight);

    this.scene.add(ambientLight);
    this.scene.add(directionalLight);
    this.scene.add(this.spotlight);
    // this.scene.add(this.spotlightHelper);
  }

  private updateCamera2Position(): void {
    if (this.rooms.length > 0) {
      const lastRoom = this.rooms[this.rooms.length - 1];

      this.camera2.position.set(
        lastRoom.position.x + 160,
        60,
        lastRoom.position.z + 100
      );
      this.camera2.lookAt(lastRoom.position);
    }
  }

  private render() {
    requestAnimationFrame(() => this.render());

    const width = this.canvas.clientWidth;
    const height = this.canvas.clientHeight;

    this.activeControls.controls.update();

    this.renderer.setViewport(0, 0, width, height);
    this.renderer.setScissor(0, 0, width, height);
    this.renderer.setScissorTest(true);
    this.renderer.render(this.scene, this.camera);

    const smallViewportWidth = width / 4;
    const smallViewportHeight = height / 4;
    const offsetX = width - smallViewportWidth;

    this.renderer.setViewport(
      offsetX,
      0,
      smallViewportWidth,
      smallViewportHeight
    );
    this.renderer.setScissor(
      offsetX,
      0,
      smallViewportWidth,
      smallViewportHeight
    );
    this.renderer.setScissorTest(true);
    this.renderer.render(this.scene, this.camera2);
  }

  private createCamera(aspectRatio: any) {
    return new THREE.PerspectiveCamera(
      this.fieldOfView,
      aspectRatio,
      this.nearClippingPlane,
      this.farClippingPlane
    );
  }

  private renderScene() {
    const aspectRatio = this.getAspectRatio();

    this.camera = this.createCamera(aspectRatio);
    this.camera.position.set(-160, 60, 100);
    this.camera.lookAt(new THREE.Vector3(0, 0, 0));

    this.camera2 = this.createCamera(aspectRatio);
    this.camera2.position.set(100, 140, 100);
    this.camera2.lookAt(new THREE.Vector3(0, 0, 0));

    this.renderer = new THREE.WebGLRenderer({ canvas: this.canvas });
    this.renderer.setPixelRatio(devicePixelRatio);
    this.renderer.setSize(this.canvas.clientWidth, this.canvas.clientHeight);
    this.renderer.shadowMap.enabled = true;
    this.renderer.shadowMap.type = THREE.PCFSoftShadowMap;

    this.setupControls();

    this.render();
  }

  private handleRoomClick(event: MouseEvent) {
    if (this.isInteractionDisabled || this.appointmentsVisible) {
      return;
    }

    for (const room of this.rooms) {
      const interactionHandled = room.onRoomClick(
        event,
        this.camera,
        this.controls1.controls
      );

      if (interactionHandled) {

        const roomPosition = room.getTablePosition();


        this.spotlight.animateSpotlight(
          this.spotlight.spotlight.position.clone(), // Current position
          new THREE.Vector3(roomPosition.x + 5, roomPosition.y + 20, roomPosition.z - 10), // New position
          this.spotlight.spotlight.target.position.clone(), // Current target
          new THREE.Vector3(roomPosition.x + 5, roomPosition.y + 2, roomPosition.z - 10) // New target
        );
      }
      this.unpickRoom(interactionHandled, room);
    }
  }

  private unpickRoom(interactionHandled: boolean, room: RoomComponent) {
    if (interactionHandled && room !== this.currentPickedRoom) {
      if (this.currentPickedRoom) {
        this.currentPickedRoom.unpickRoom();
      }
      this.currentPickedRoom = room;
      this.currentRoomName = room.roomName;
    }
  }

  private filterAppointments(): void {
    this.filteredAppointments = this.apList.filter(
      (appointment) =>
        String(appointment.surgeryRoomNumber) === String(this.currentRoomName)
    );
  }

  private getAppointmentsDatesForRoom(roomName: string): AppointmentDates[] {
    const filteredAppointments = this.apList.filter(
      (appointment) =>
        String(appointment.surgeryRoomNumber) === String(roomName)
    );

    return filteredAppointments.map((appointment) => {
      const startDateTime = appointment.dateTime;
      const endDateTime = this.calculateEndDateTime(appointment);

      return {
        startDateTime,
        endDateTime,
      };
    });
  }


  closeModal() {
    this.appointmentsVisible = false;
    this.isInteractionDisabled = true;
    setTimeout(() => {
      this.isInteractionDisabled = false;
    }, 1);
  }

  constructor(
    private service: AppointmentService,
    private route: ActivatedRoute
  ) { }

  async ngOnInit(): Promise<void> {
    const date = this.route.snapshot.queryParamMap.get('date');

    if (date) {
      await this.getAppointmentList(date);
    }

    await fetch('/assets/json/rooms.json')
      .then((response) => response.json())
      .then((data) => {
        this.roomsJson = data;

        this.createScene();
        this.renderScene();

        this.clickEvent();

        this.eventKey();
      })
      .catch((error) => {
        console.error('Error loading rooms data:', error);
      });
  }

  private async getAppointmentList(date: any): Promise<void> {
    return new Promise((resolve, reject) => {
      this.service.getAppointmentList(date).subscribe(
        (ap) => {
          this.apList = ap;
          resolve();
        },
        (error) => {
          resolve();
        }
      );
    });
  }

  private clickEvent() {
    window.addEventListener('click', (event) => this.handleRoomClick(event));
  }

  private eventKey() {
    window.addEventListener('keydown', (event) => {
      if (event.key === 'c') {
        this.toggleControl(); // Switch camera controls
      }
      if (event.key === 'i') {
        if (this.appointmentsVisible) {
          this.closeModal();
        } else {
          this.filterAppointments();
          this.appointmentsVisible = true;
        }
      }
    });
  }

  calculateEndDateTime(appointment: any): string {
    if (!appointment?.dateTime || !appointment?.operationRequest?.opTypeName) {
      return 'Invalid Data';
    }

    const { cleaningInMinutes = 0, anesthesiaPatientPreparationInMinutes = 0, surgeryInMinutes = 0 } =
      appointment.operationRequest.opTypeName;

    const dateParts = appointment.dateTime.split(/[-/ :]/);
    const initialDate = new Date(
      parseInt(dateParts[2]),
      parseInt(dateParts[1]) - 1,
      parseInt(dateParts[0]),
      parseInt(dateParts[3] || '0'),
      parseInt(dateParts[4] || '0'),
      parseInt(dateParts[5] || '0')
    );

    const totalMinutes = cleaningInMinutes + anesthesiaPatientPreparationInMinutes + surgeryInMinutes;
    const endDate = new Date(initialDate.getTime() + totalMinutes * 60000);

    const date = new Intl.DateTimeFormat('en-GB', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
    }).format(endDate);

    const time = new Intl.DateTimeFormat('en-GB', {
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit',
      hour12: false,
    }).format(endDate);

    return `${date} ${time}`;
  }



}
