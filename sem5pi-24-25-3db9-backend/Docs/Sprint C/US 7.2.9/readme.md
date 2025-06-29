# US 7.2.9

## 1. Context

This task appears in the end of the project's development, to be able to edit a new appointment.


## 2. Requirements

**US 7.2.9** As a Doctor, I want to update a Surgery Appointment, so that I can override the automatically generated planning.

**Acceptance Criteria:**

**Dependencies/References:**

It is also required that the user is registered and logged in as a doctor.


## 3. Analysis

For this US were considered the requirements specified in the project's description and the client's answers. 
Some relevant answers excerpts are here specified:

- **Question:** As a follow-up question, what exactly can the doctor update about the appointment? Can they, for example, change the surgery room for the surgery?
  - **Answer:** after the appointment is planned, it is possible to update the team, room and date. the system must ensure all the resources and personnel is available at the selected time according to the operation type duration.

- **Question:** According to a previous answer about this requirement, when the doctor attempts the creation of an appointment, they specify room, date and team. But do they also specify the time in which the surgery should start?
  - **Answer:** yes

The following **HTTP requests** will be implemented:
- PATCH (to edit a new appointment)


## 4. Design

This section presents the design adopted to solve the requirement.

### 4.1. Level 1 Sequence Diagram

This diagram guides the realization of the functionality, for level 1 procecss view.

![US 7.2.9 N1](US%207.2.9%20N1.svg)


### 4.2. Level 2 Sequence Diagram

This diagram guides the realization of the functionality, for level 2 procecss view.

![US 7.2.9 N2](US%207.2.9%20N2.svg)


### 4.3. Level 3 Sequence Diagram

This diagram guides the realization of the functionality, for level 3 process view, for both backend and frontend.

**Backend**

![US 7.2.9 N3](US%207.2.9%20N3.svg)

**Frontend**
![US 7.2.9 N3 SPA](US%207.2.9%20N3%20SPA.svg)

### 4.4. Applied Design Patterns

- **Domain Driven Development (DDD):** the focus is the business logic and not the implementation.
- **Data Transfer Object (DTO):** gives an abstraction layer to the domain, so that it's only presented specific information regarding the object.
- **Model View Controller (MVC):** allows the re-usability of components and promotes a more modular approach to the code, making it easier to manage and maintain.
- **Repository pattern:** allows access to data without sharing the details of data storing, like the database connection.
- **Service pattern:** helps keeping high cohesion and low coupling in the code by separating complex business logic from the rest of the system. They also promote reuse, as multiple parts of the system can use the same service to perform common operations.
- **Test Driven Development (TDD):** planning the tests previously to the code gives orientation lines to the development process.
- **Onion Architecture:** concentric layers structure that puts the Domin Model as the core. Promotes modularity, flexibility and testability.
- **Inversion of Control:** the responsability of object creation and dependency management belongs to a framework or external entity, so that the class doesn't need to. Promotes flexibility and decoupling.
- **Dependency Injection:** used to implement inversion of control. The dependencies are injected into a class from the outside.


### 4.5. Tests

#### 4.5.1. Unit and Integration Tests

- Unit Tests for appointment entity with valid inputs

## 5. Implementation

The implementation of this US is according to the design, as can be seen in the SD and CD presented before.

All commits referred the corresponding issue in GitHub, using the #14 and #19 tag, as well as a relevant commit message.


## 6. Integration/Demonstration

To edit a appointment, run the Backoffice, Auth and SPA app then go to the appointment page and click on the pencil icon next to a appointment to edit a appointment.

## 7. Observations

This work was guided by the project provided in ARQSI classes.