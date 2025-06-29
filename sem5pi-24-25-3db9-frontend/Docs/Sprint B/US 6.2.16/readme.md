# US 6.2.16

## 1. Context

This task appears in the middle of the project's development, to be able to deactivate operation requests.


## 2. Requirements

**US 6.2.16**  As a Doctor, I want to remove an operation requisition, so that the healthcare activities are provided as necessary.

**Acceptance Criteria:**

- Doctors can delete operation requests they created if the operation has not yet been
scheduled.
- A confirmation prompt is displayed before deletion.
- Once deleted, the operation request is removed from the patient’s medical record and cannot
be recovered.
- The system notifies the Planning Module and updates any schedules that were relying on this
request.

**Dependencies/References:**

The user logged in must be registered in the system as a 'Doctor'.

## 3. Analysis

The following requirements specified by the client were considered during the development of this user story:

- **Question:** Já referiu que os registos do medical record são adicionados manualmente. Apagar esses registos também o deverá ser? Mais especificamente na Us 5.1.18 diz "Once deleted, the operation request is removed from the patient’s medical record and cannot
be recovered". Este delete do medical record deverá ser manual ou deverá acontecer ao mesmo tempo do delete do operation request?
  - **Answer:** O sistema nao faz a ligação automatica entre medical history e operation request.

### 3.2. HTTP requests

The following **HTTP requests** will be implemented:
- GET by ID (to check a specific operation request)
- DELETE (to deactivate the operation request)

## 4. Design

This section presents the design adopted to solve the requirement.

### 4.1. Level 1 Sequence Diagram

This diagram guides the realization of the functionality, for level 1 process view.

![US6.2.16 N1 SD](US6.2.16%20N1%20SD.svg)


### 4.2. Level 2 Sequence Diagram

This diagram guides the realization of the functionality, for level 2 process view.

![US6.2.16 N2 SD](US6.2.16%20N2%20SD.svg)


### 4.3. Level 3 Sequence Diagram

This diagram guides the realization of the functionality, for level 3 process view.

![US6.2.16 N3 SD](US6.2.16%20N3%20SD.svg)


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

The following tests are to be developed:
- deactivating an operation request hides its data.
- a success message should be displayed.
- the page must reload to show the deleted operation request.


Unitary tests to the component will be implemented.

E2E Tests will also check all the US flow, from the user interaction to the backend server.


## 5. Implementation

The implementation of this US is according to the design, as can be seen in the diagrams presented before.

All commits referred the corresponding issue in GitHub, using the #39 tag, as well as a relevant commit message.

The estimated duration for this US is 12h.


## 6. Integration/Demonstration

To deactivate an operation request, run the Backoffice, Auth and the SPA app, then go to the Operation Request page and click on the delete icon in the operation request you wish to delete.

## 7. Observations

This work was guided by the project provided in ARQSI classes.