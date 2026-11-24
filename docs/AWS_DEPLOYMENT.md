# Optional AWS deployment guide

Status: instructions only. No AWS account was accessed, resources created, charges incurred or deployment claimed. The user requested local PostgreSQL first. The working local version remains the classroom demonstration target.

## Smallest relevant architecture

eTulong desktop -> HTTPS ASP.NET Core API on an EC2 instance -> private Amazon RDS for PostgreSQL. All clients use the same database authority. The conditional capacity update, unique reservation and transaction remain necessary on AWS: hosting alone does not prevent overbooking. RDS Proxy is optional connection-pooling infrastructure, not a replacement for correct transaction logic.

For the closest deployment to the included binaries, use a Windows x64 EC2 host for publish/Api. A Linux host requires separately publishing Api for linux-x64 and verifying that build; the provided win-x64 apphost cannot run on Linux. The desktop remains a Windows application on office workstations.

## Deployment sequence to review with the instructor

1. Confirm the approved scope, AWS account/budget, region and whether a cloud demo is actually required. Use fictional data. No specific paid instance tier is assumed or quoted here.
2. Create the EC2 API host and RDS PostgreSQL instance in a suitable VPC. Keep RDS private. Allow inbound PostgreSQL5432 only from the API host's security group, not from every desktop or the public internet. Restrict host administration to trusted access.
3. Create a dedicated application database/role. Store credentials outside source and submitted archives. Configure the API host's ETULONG_DB with the RDS endpoint, database and application role. Use a trusted RDS CA and full server-certificate verification, for example Npgsql options SSL Mode=VerifyFull;Root Certificate=C:\secure\rds-ca-bundle.pem. Do not disable certificate validation.
4. Copy the entire compatible API publish folder. Run Api.exe --initialize using a controlled migration credential to apply the included migration/reference seed. Verify that existing operational records remain intact. Run the serving process as a managed service with restricted privileges and an appropriate runtime configuration.
5. Expose the API through a properly configured HTTPS endpoint/reverse proxy and restrict demo access to the approved network. Bind the internal API appropriately behind that proxy. Do not expose the current demonstration-operator API publicly as a real assistance service.
6. Configure the desktop ApiBaseAddress to the HTTPS endpoint, or use ETULONG_API. No database credential belongs in desktop settings. Verify health, both workflows, rejection/rollback and restart behavior against the cloud environment.
7. Re-run the concurrency scenario with two clients against the same RDS database and inspect saved capacity/reservations. Retain actual results; do not reuse local evidence as proof of an AWS execution.
8. Enable appropriate backups/logging and document restoration and connectivity behavior. Cloud-hosted records require internet connectivity in this implementation. There is no offline synchronization mechanism.

## Before any public or real-data deployment

Replace X-Demo-Staff with authenticated identities and server-side authorization. A selectable demo identity is not proof of who is making a request. Review supported runtime and package versions, access controls, data retention, consent, image scanning/storage protection and operational support. The classroom build targets .NET8/EF Core8 because it reuses the established course toolchain; upgrade to a maintained supported stack before production hosting. Keep real eligibility decisions with authorized human officials. None of this production work is claimed complete by the local prototype.

## Official references checked for this guide

- [AWS RDS public/private access](https://docs.aws.amazon.com/AmazonRDS/latest/gettingstartedguide/security-public-private.html)
- [Connect an EC2 host and an RDS database](https://docs.aws.amazon.com/AmazonRDS/latest/UserGuide/ec2-rds-connect.html)
- [RDS PostgreSQL TLS and certificate verification](https://docs.aws.amazon.com/AmazonRDS/latest/UserGuide/PostgreSQL.Concepts.General.SSL.html)
- [EC2 security groups](https://docs.aws.amazon.com/AWSEC2/latest/UserGuide/creating-security-group.html)
- [RDS Proxy use cases and query-duration limitations](https://docs.aws.amazon.com/AmazonRDS/latest/UserGuide/rds-proxy-best-practices.usage-scenarios.html)
- [PostgreSQL concurrent update semantics](https://www.postgresql.org/docs/16/transaction-iso.html)
