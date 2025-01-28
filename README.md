# Kafka KRaft TEST

![Kafka Logo](https://img.shields.io/badge/Apache_Kafka-231F20?style=for-the-badge&logo=apache-kafka&logoColor=white)

A test application demonstrating Kafka's fault tolerance capabilities using KRaft (Kafka Raft Metadata) mode without ZooKeeper dependency.

## Steps to Run

1. Generate UUID for the cluster:
   ```bash
   docker run --rm apache/kafka:3.9.0 /opt/kafka/bin/kafka-storage.sh random-uuid
   ```
   or
   ```bash
    # macos
    uuidgen | tr -d '-' | base64 | cut -c 1-22

    # linux
    cat /proc/sys/kernel/random/uuid | tr -d '-' | base64 | cut -b 1-22
   ```

   Copy the generated UUID and use it in the docker-compose.yml file as the CLUSTER_ID. 
   ```bash
   CLUSTER_ID: "UUID-GENERATED"
   ```

2. Start the Kafka cluster:
   ```bash
   docker compose up -d
   ```
   This will start 3 Kafka brokers in KRaft mode.

3. Create a test topic:
   ```bash
   docker exec kafka1 kafka-topics \
    --bootstrap-server kafka1:9092 \
    --create \
    --topic "TOPIC-NAME" \
    --partitions 3 \
    --replication-factor 3 \
    --config min.insync.replicas=2
   ```

4. Start the producer and consumer inside dev container:
  ```bash
    # producer
    cd app/producer && dotnet run
  ```

  ```bash
    # consumer
    cd app/consumer && dotnet run
  ```

5. List topics to verify creation:
  - producer: enter the "TOPIC-NAME" previously created
  - consumer: enter the "TOPIC-NAME" previously created

6. Test fault tolerance:
  - Stop one of the brokers:
    ```bash
     docker stop kafka3
    ```
  - Verify the cluster continues to operate with the remaining brokers
  - Restart the stopped broker:
    ```bash
      docker start kafka3
    ```

7. Clean up:
  ```bash
    docker compose down -v
  ```
