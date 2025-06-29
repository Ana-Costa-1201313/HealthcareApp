# US 6.2.17

## 1. Context

This task appears in the middle of the project's development, to be able to list operation requests.


## 2. Requirements

**US 6.2.17**  As a Doctor, I want to list/search operation requisitions, so that I see the details, edit, and remove operation requisitions.

**Acceptance Criteria:**

- Doctors can search operation requests by patient name, operation type, priority, and status.
- The system displays a list of operation requests in a searchable and filterable view.
- Each entry in the list includes operation request details (e.g., patient name, operation type,
status).
- Doctors can select an operation request to view, update, or delete it.

**Dependencies/References:**

The user logged in must be registered in the system as a 'Doctor'.

## 3. Analysis

The following requirements specified by the client were considered during the development of this user story:

- **Question:** In the acceptance criteria for US19 - "As a Doctor, I want to list/search operation requisitions, so that I can see the details, edit, and remove operation requisitions," one of the criteria specifies: "- The system displays a list of operation requests in a searchable and filterable view."
Could you please clarify which filters the doctor can apply to the Operation Requisition search?
  - **Answer:** The doctor can search and filter by operation type, patient name, patient medical record number, date range.

- **Question:** One of the acceptance criteria mentions that physicians can search for operating requisitions by status.
What does this refer to in context?
  - **Answer:** Yes, whether the operation is already planned or not. Those are basically the two statuses that you will have. So, you create an operation request, and it is requested. But then it will be passed to the planning model and the planning model will pick those requests and create the support for the operations. So the request that you are making at this moment becomes picked. So you can add and look for which are my requests that are already picked.

  - **Question:** When listing operation requests, should only the operation requests associated to the logged-in doctor be displayed?
  - **Answer:** A doctor can see the operation requests they have submitted as well as the operation requests of a certain patient.
An Admin will be able to list all operation requests and filter by doctor
it should be possible to filter by date of request, priority and expected due date.

### 3.2. HTTP requests

The following **HTTP requests** will be implemented:
- GET (with query parameters, to check specific operation requests)

## 4. Design

This section presents the design adopted to solve the requirement.

### 4.1. Level 1 Sequence Diagram

This diagram guides the realization of the functionality, for level 1 process view.

![US6.2.17 N1 SD](US6.2.17%20N1%20SD.svg)


### 4.2. Level 2 Sequence Diagram

This diagram guides the realization of the functionality, for level 2 process view.

![US6.2.17 N2 SD](US6.2.17%20N2%20SD.svg)


### 4.3. Level 3 Sequence Diagram

This diagram guides the realization of the functionality, for level 3 process view.

![US6.2.17 N3 SD](US6.2.17%20N3%20SD.svg)


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
- all operation requests must be displayed in the operation request page.
- getting a operation request by operation type name, patient email, doctor email, priority, deadline date or status.

Unitary tests to the component will be implemented.

E2E Tests will also check all the US flow, from the user interaction to the backend server.


## 5. Implementation

The implementation of this US is according to the design, as can be seen in the diagrams presented before.

All commits referred the corresponding issue in GitHub, using the #40 tag, as well as a relevant commit message.

The estimated duration for this US is 21h.


## 6. Integration/Demonstration

To list operation requests, run the Backoffice, Auth and the SPA app, then go to the Operation Request page and check all shown operation requests. Choose one request to see the details. Choose a filter if desired.

## 7. Observations

This work was guided by the project provided in ARQSI classes.