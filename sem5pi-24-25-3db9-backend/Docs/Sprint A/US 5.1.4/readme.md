# US 5.1.4

## 1. Context

This task appears in the start of the project's development, to be able to update a profile as a patient.


## 2. Requirements

**US 5.1.4** As a Patient, I want to update my user profile, so that I can change my personal details and preferences.

**Acceptance Criteria:**

- Patients can log in and update their profile details (e.g., name, contact information, preferences).
- Changes to sensitive data, such as email, trigger an additional verification step (e.g., confirmation email).
- All profile updates are securely stored in the system.
- The system logs all changes made to the patient's profile for audit purposes.


**Dependencies/References:**

The user logged in must be registered in the system as a 'Patient'.

## 3. Analysis

The following requirements specified by the client were considered during the development of this user story:

- **Question:** My question is whether patients can also update patient profile information in addition to user profile information.
And if so, is there any information in the patient profile that can be updated?
  - **Answer:**  If we're talking about contact information, yes. Okay? So the patient, if he has a user in the system, he should be able to at least activate, as you can see in the second acceptance criteria, to activate the update. For example, if I want to change my phone number or if I want to change my email, then that cannot be changed directly through my user contact. It needs to be verified and validated. That is what is expected. You cannot change anything related to the medical information.

## 4. Design



### 4.1. Level 1 Sequence Diagram

![US5.1.4 N1 SD](US5.1.4%20N1%20SD.svg)

### 4.2. Level 2 Sequence Diagram

![US5.1.4 N2 SD](US5.1.4%20N2%20SD.svg)

### 4.3. Level 3 Sequence Diagram

![US5.1.4 N3 SD](US5.1.4%20N3%20SD.svg)

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




## 5. Implementation

The implementation of this US is according to the design, as can be seen in the diagrams presented before.

All commits referred the corresponding issue in GitHub, using the #19 tag, as well as a relevant commit message.


## 6. Integration/Demonstration


## 7. Observations

This work was guided by the project provided in ARQSI classes.