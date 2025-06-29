# US 7.2.22

## 1. Context

This task appears in the end of the project's development, to be able to create a free text entry in a patient medical record.


## 2. Requirements

**US 7.2.22** As a Doctor, I want to add a free text entry  in the medical record of a patient.

**Acceptance Criteria:**

**Dependencies/References:**

It is also required that the user is registered and logged in as a doctor.


## 3. Analysis

### 3.1. HTTP requests

The following **HTTP requests** will be implemented:
- POST (to register the free text alongside the patient record)

## 4. Design

This section presents the design adopted to solve the requirement.

### 4.1. Level 1 Sequence Diagram

This diagram guides the realization of the functionality, for level 1 procecss view.

![US 7.2.22 N1](US7.2.22%20N1%20SD.png)


### 4.2. Level 2 Sequence Diagram

This diagram guides the realization of the functionality, for level 2 procecss view.

![US 7.2.22 N2](US7.2.22%20N2%20SD.png)


### 4.3. Level 3 Sequence Diagram

This diagram guides the realization of the functionality, for level 3 process view, for both backend and frontend.

**Backend**

![US 7.2.22 N3](US7.2.22%20N3%20SD.png)

**Frontend**
![US 7.2.22 N3 SPA](US7.2.22%20N3%20SD%20SPA.png)



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

All Value Objects will be tested in Unitary Tests, to check if they fullfill their requirements.

The patient record Service will be tested to see if the created patient record is correct.

The patient record Controller will be tested to see if the created patient record and responses are correct.


## 5. Implementation

The implementation of this US is according to the design, as can be seen in the diagrams presented before.

All commits referred the corresponding issue in GitHub, using the #43 and #56 tags, as well as a relevant commit message.


## 6. Integration/Demonstration

To register a new free text entry, run the RecordsBackoffice and SPA apps and send a POST HTTP request with the new data.

## 7. Observations

This work was guided by the project provided in ARQSI classes.