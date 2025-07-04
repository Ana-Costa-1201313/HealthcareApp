# US 7.2.16

## 1. Context

This task appears in the end of the project's development, to be able to edit an allergy.


## 2. Requirements

**US 7.2.16** As a Doctor, I want to edit Allergies, so that I can use it to update the Patient Medical Record.

**Acceptance Criteria:**

- Doctors can edit allergy details such as the name, code and optional longer description.
- The profile is stored securely, and access is based on role-based permissions.

**Dependencies/References:**

It is also required that the user is registered and logged in as an admin.


## 3. Analysis

For this US were considered the requirements specified in the project's description and the client's answers. 
Some relevant answers excerpts are here specified:

- **Question:** What information is to be known in an Allergy? Like designation, and anything more?
  - **Answer:** It consist of a code (for instance, SNOMED CT (Systematized Nomenclature of Medicine - Clinical Terms) or ICD-11 (International Classification of Diseases, 11th Revision)), a designation and an optional longer description.


### 3.1. Domain Model excerpt
![DM allergy](DM%20allergy.png)


### 3.2. HTTP requests

The following **HTTP requests** will be implemented:
- PATCH (to edit the allergy)

## 4. Design

This section presents the design adopted to solve the requirement.

### 4.1. Level 1 Sequence Diagram

This diagram guides the realization of the functionality, for level 1 procecss view.

![US 7.2.16 N1](US%207.2.16%20N1.svg)


### 4.2. Level 2 Sequence Diagram

This diagram guides the realization of the functionality, for level 2 procecss view.

![US 7.2.16 N2](US%207.2.16%20N2.svg)


### 4.3. Level 3 Sequence Diagram

This diagram guides the realization of the functionality, for level 3 process view, for both backend and frontend.

**Backend**

![US 7.2.16 N3](US%207.2.16%20N3.svg)

**Frontend**
![US 7.2.16 N3 SPA](US%207.2.16%20N3%20SPA.svg)



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
- The name must be unique.
- The name is required.
- The code must be unique.
- The code is required.
- The description can be null.

All Value Objects will be tested in Unitary Tests, to check if they fullfill their requirements.

The allergy Service will be tested to see if the gotten allergy is correct.

The allergy Controller will be tested to see if the result allergy and responses are correct.


## 5. Implementation

The implementation of this US is according to the design, as can be seen in the diagrams presented before.

All commits referred the corresponding issue in GitHub, using the #50 tag, as well as a relevant commit message.


## 6. Integration/Demonstration

To register a new allergy, run the RecordsBackoffice app and send a PATCH HTTP request with the new data.

## 7. Observations

This work was guided by the project provided in ARQSI classes.