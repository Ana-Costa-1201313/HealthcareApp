# US 7.2.8

## 1. Context

This task appears in the end of the project's development, to be able to create a surgery appointment.


## 2. Requirements

**US 7.2.8** As a Doctor, I want to create a Surgery Appointment, so that the Patient doesn’t need to wait for the automatically generated planning.

**Acceptance Criteria:**

**Dependencies/References:**

It is also required that the user is registered and logged in as a doctor.


## 3. Analysis

For this US were considered the requirements specified in the project's description and the client's answers. 
Some relevant answers excerpts are here specified:

- **Question:** Should the doctor be able to make a surgery appointment without making a prior operation request for said appointment?
  - **Answer:** The doctor must be able to "transform" an existing operation request into an actual appointment by specifying the room, date and team of the surgery. the system must ensure all the resources and personnel is available at the selected time according to the operation type duration.

- **Question:** Regarding the team selected by the doctor when creating the appointment, does this team include only doctors, doctors and anesthetists, or doctors, anesthetists and cleaners?
  - **Answer:** it must include the whole team that conforms to the team composition according to the operation type specification

- **Question:** When the doctor selects the team for the Appointment, that includes doctors and nurses. Regarding cleaners, which staff (doctors, nurses, interns) can be selected for that role? And is there any criteria for that selection?
  - **Answer:** the doctor only selects the medical team

- **Question:** When a doctor is selecting the staff for an appointment, what should happen if, for every slot he could choose, there aren't enough staffs to perform the operation?
  - **Answer:** the appointment cannot be scheduled for that date. the doctor must choose a different date


### 3.1. HTTP requests

The following **HTTP requests** will be implemented:
- POST (to register the new appointment)

## 4. Design

This section presents the design adopted to solve the requirement.

### 4.1. Level 1 Sequence Diagram

This diagram guides the realization of the functionality, for level 1 procecss view.

![US 7.2.8 N1](US%207.2.8%20N1.svg)


### 4.2. Level 2 Sequence Diagram

This diagram guides the realization of the functionality, for level 2 procecss view.

![US 7.2.8 N2](US%207.2.8%20N2.svg)


### 4.3. Level 3 Sequence Diagram

This diagram guides the realization of the functionality, for level 3 process view, for both backend and frontend.

**Backend**

![US 7.2.8 N3](US%207.2.8%20N3.svg)

**Frontend**
![US 7.2.8 N3 SPA](US%207.2.8%20N3%20SPA.svg)



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

The appointment Service will be tested to see if the created appointment is correct.

The appointment Controller will be tested to see if the created appointment and responses are correct.


## 5. Implementation

The implementation of this US is according to the design, as can be seen in the diagrams presented before.

All commits referred the corresponding issue in GitHub, using the #13 and #18 tags, as well as a relevant commit message.


## 6. Integration/Demonstration

To register a new surgery appointment, run the Backoffice app and send a POST HTTP request with the new data.

## 7. Observations

This work was guided by the project provided in ARQSI classes.