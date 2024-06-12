import styled from 'styled-components/native';

export default DiaryWorkoutContainer = styled.View`
    flex-direction: row;
    width: 100%;
    height: 55%;
    margin-top: ${props => props.marginTop};
    gap: ${props => props.gap};
`;
