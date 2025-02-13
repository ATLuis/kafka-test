# Kafka Testing
This repository was created to properly configure Kafka for high availability. The objective is to disconnect Kafka brokers one by one, ensuring the application continues running without issues.

## Test Scenarios

1. **Kafka1 & Kafka2 Running**
    - Expected behavior: Application continues running
    - Actual behavior: Application continues running
    - **Test Result**: ✅ Passed

2. **Kafka1 Running & Kafka2 Stopped**
    - Expected behavior: Application continues running
    - Actual behavior: Application continues running
    - **Test Result**: ✅ Passed

3. **Kafka1 Stopped & Kafka2 Running**
    - Expected behavior: Application continues running
    - Actual behavior: Application continues running
    - **Test Result**: ✅ Passed

4. **Kafka1 & Kafka2 Stopped**
    - Expected behavior: Application crashes
    - Actual behavior: Application crashes
    - **Test Result**: ✅ Passed

5. **Kafka1 Running & Kafka2 Stopped**
    - Expected behavior: Application continues running
    - Actual behavior: Application crashes
    - **Test Result**: ❌ Failed

6. **Kafka1 Stopped & Kafka2 Running**
    - Expected behavior: Application continues running
    - Actual behavior: Application crashes
    - **Test Result**: ❌ Failed

7. **Kafka1 & Kafka2 Running**
    - Expected behavior: Application continues running
    - Actual behavior: Application crashes
    - **Test Result**: ❌ Failed
