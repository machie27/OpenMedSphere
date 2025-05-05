# OpenMedSphere: Secure Medical Research Collaboration Platform
I'd like to begin by saying that, while I've always had a fascination with medical care, I'm definitely not a doctor and do not claim any medical expertise. However, I have a deep passion for new technologies and have always dreamt of making a positive impact. Please note that this project is primarily designed as a personal learning experience for me, aimed at exploring and mastering new technologies. Additionally, I'm keen to challenge myself by integrating industry-standard guidelines and practices, which explains why I've introduced these somewhat ambitious requirements.

## Vision:

Create an open-source, secure, scalable, and collaborative platform specifically tailored for medical researchers to securely process, share, and visualize anonymized patient data, leveraging cutting-edge quantum-safe encryption and cloud-native technologies. The platform should be easy to self-host, allowing institutions and researchers full control over their data and infrastructure.

## Technology Stack:

- **Framework:** .NET Core Aspire (with consideration for ease of self-hosting)
    
- **Architecture:** Domain-Driven Design (DDD), Clean Architecture
    
- **CI/CD:** GitHub Pipelines
    
- **Encryption:** OpenPGP integrated with Quantum-safe encryption (e.g., CRYSTALS-Kyber, Falcon) // TODO: definitely look into these Quantum-safe encryption technologies, since you don't understand them!!!!
    

## Core Features:

1. **Distributed Data Processing:**
    
    - Utilize Aspire's native microservices to build scalable distributed processing clusters.
        
    - Integrate message queues (RabbitMQ or Kafka via Aspire, other messaging options should also be explored lightly) for efficient asynchronous handling of extensive datasets.
        
2. **Secure Sharing of Anonymized Data:**
    
    - Adhere to strict anonymization standards ensuring HIPAA and GDPR compliance.
        
    - Employ OpenPGP combined with quantum-safe encryption for secure data at rest and in transit.
        
    - Maintain comprehensive audit trails and detailed logging to track secure data access.
        
3. **Visualization Tools:**
    
    - Develop interactive data visualizations using frontend frameworks like Blazor or React (Vue?).
        
    - Offer customizable dashboards presenting insights derived from research data.
        

## Project Structure (Clean Architecture & DDD):

- **Domain Layer:** Captures core business logic with entities like `PatientData`, `ResearchStudy`, and `AnonymizationPolicy`.
    
- **Application Layer:** Implements core use cases and domain orchestrations.
    
- **Infrastructure Layer:** Manages database interactions (SQL/NoSQL), external integrations, and encryption services.
    
- **Presentation Layer:** Provides API endpoints and visualization tools.
    

## GitHub Pipelines (CI/CD):

- Automate build processes, unit/integration testing, and deployments both to cloud providers (Azure/AWS) and local server environments.
    
- Integrate security scanning (CodeQL), dependency management (Dependabot), and encryption validation.
    
- Secure, reliable deployment via GitHub Actions. (future) 
    

## Encryption Strategy:

- Evaluate and select quantum-safe cryptographic standards such as CRYSTALS-Kyber. // TODO: look into quantum-safe cryptographic standards a bit more, lacking real-time knowlegde here as well)
    
- Implement an abstracted encryption service facilitating seamless future upgrades.
    

#### Open Source Licensing and Management:

- Licensed under GNU Affero General Public License v3 (AGPL v3) to ensure openness, collaboration, and protection from unauthorized commercial exploitation.
    
- Comprehensive documentation, clear contribution guidelines, and active community engagement.
    
- Public project roadmap and transparent issue tracking through GitHub.
    

## Initial Steps:

- [ ] Define a bit more that this is also a project for myself to work with new technologies and will main focus on this instead

- [ ] Set up the initial repository with CI/CD workflows.

- [ ] Initialize basic Aspire microservices with clear documentation on self-hosting setup.

- [ ] Establish initial domain models following DDD.

- [ ] Implement the foundational encryption layer.
