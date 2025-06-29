# US 5.1.16

## 1. Context

This task appears in the start of the project's development, to be able to request an operation.


## 2. Requirements

**US 5.1.16** As a Doctor, I want to request an operation, so that the Patient has access to the necessary healthcare.

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

## 4. Design

### 4.1. Level 1 Sequence Diagram

![US5.1.16 N1 SD](US5.1.16%20N1%20SD.svg)

### 4.2. Level 2 Sequence Diagram

![US5.1.16 N2 SD](US5.1.16%20N2%20SD.svg)

### 4.3. Level 3 Sequence Diagram

![US5.1.16 N3 SD](US5.1.16%20N3%20SD.svg)

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

The OperationRequest Controller will be tested to check if the request is properly created and if the responses are correct.

## 5. Implementation

The implementation of this US is according to the design, as can be seen in the diagrams presented before.

All commits referred the corresponding issue in GitHub, using the #19 tag, as well as a relevant commit message.


## 6. Integration/Demonstration

To register a new operation request, run the Backoffice app and send a POST HTTP request with the new operation request data.

## 7. Observations

This work was guided by the project provided in ARQSI classes.