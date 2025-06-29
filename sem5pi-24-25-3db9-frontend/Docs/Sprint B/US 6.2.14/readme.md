# US 6.2.14

## 1. Context

This task appears in the middle of the project's development, to be able to create operation requests.


## 2. Requirements

**US 6.2.14**  As a Doctor, I want to request an operation, so that the Patient has access to the necessary healthcare.

**Acceptance Criteria:**

- Doctors can create an operation request by selecting the patient, operation type, priority, and suggested deadline.
- The system validates that the operation type matches the doctor’s specialization.
- The operation request includes:
  - Patient ID
  - Doctor ID
  - Operation Type
  - Deadline
  - Priority
- The system confirms successful submission of the operation request and logs the request in the patient’s medical history.


**Dependencies/References:**

The user logged in must be registered in the system as a 'Doctor'.

## 3. Analysis

The following requirements specified by the client were considered during the development of this user story:

- **Question:** Does the system adds automically the operation request to the medical history of the patient?
  - **Answer:** No need. it will be the doctor's responsibility to add it.

- **Question:** Can a doctor make more than one operation request for the same patient? If so, is there any limit or rules to follow? For example, doctors can make another operation request for the same patient as long as it's not the same operation type?
  - **Answer:** It should not be possible to have more than one "open" surgery request (that is, a surgery that is requested or scheduled but not yet performed) for the same patient and operation type.


### 3.2. HTTP requests

The following **HTTP requests** will be implemented:
- POST (to register the new operation request)
- GET (to check the new operation request)

## 4. Design

This section presents the design adopted to solve the requirement.

### 4.1. Level 1 Sequence Diagram

This diagram guides the realization of the functionality, for level 1 process view.

![US6.2.14 N1 SD](US6.2.14%20N1%20SD.svg)


### 4.2. Level 2 Sequence Diagram

This diagram guides the realization of the functionality, for level 2 process view.

![US6.2.14 N2 SD](US6.2.14%20N2%20SD.svg)


### 4.3. Level 3 Sequence Diagram

This diagram guides the realization of the functionality, for level 3 process view.

![US6.2.14 N3 SD](US6.2.14%20N3%20SD.svg)


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
- the created operation request must have the same data as the input.
- an error message should be displayed when the input is invalid or mandatory.
- the page must reload to show the new operation request.
- a success message should be shown after creation.


Unitary tests to the component will be implemented.

E2E Tests will also check all the US flow, from the user interaction to the backend server.


## 5. Implementation

The implementation of this US is according to the design, as can be seen in the diagrams presented before.

All commits referred the corresponding issue in GitHub, using the #37 tag, as well as a relevant commit message.


## 6. Integration/Demonstration

To create an operation request, run the Backoffice, Auth and the SPA app, then go to the Operation Request page and click on "plus" icon to add an operation request.

## 7. Observations

This work was guided by the project provided in ARQSI classes.