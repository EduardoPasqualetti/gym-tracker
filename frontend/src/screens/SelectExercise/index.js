import React, { useEffect, useState } from 'react';
import { Title } from '../../components/Title/style';
import { Button } from '../../components/Button';
import { IconButton } from '../../components/IconButton';
import { MaterialIcons } from '@expo/vector-icons';
import { FlatList, TouchableOpacity } from 'react-native';
import BtnExercise from './style'; // Certifique-se de que o caminho está correto
import Gradient from '../../components/Gradient';
import { Container } from '../../components/Container/style';
import { ListContainer } from '../../components/ListContainer/style';
import { SelectedExerciseModal } from '../../components/SelectedExerciseModal';
import { percentage } from '../../utils/percentageFactory';
import { GetExercisesByMuscleGroupId } from '../../infra/services/exerciseService';
import { ExerciseCard } from '../../components/ExerciseCard';

export const SelectExercise = ({ navigation, route }) => {
    const [showModalExercise, setShowModalExercise] = useState(false);
    const [exercises, setExercises] = useState();
    const mucleGroupId = route.params.mucleGroupId;

    const renderItem = ({ item }) => (
        <TouchableOpacity onPress={() => setShowModalExercise(true)}>
            <ExerciseCard
                titleExercise={item.exerciseName}
                marginBottom='16px'
            />
        </TouchableOpacity>
    );

    useEffect(() => {
        GetExercises()
    }, [])

    async function GetExercises() {
        const response = await GetExercisesByMuscleGroupId(mucleGroupId);
        setExercises(response.data);
    }


    return (
        <Gradient>
            <Container>
                <IconButton
                    gradient={false}
                    icon={
                        <MaterialIcons
                            name='reply'
                            size={40}
                            color={'#FB6614'}
                            onPress={() => navigation.goBack()}
                        />
                    }
                />

                <Title
                    FontSize={20}
                    marginTop={percentage(0.12, 'h')}
                    marginBottom={percentage(0.05, 'h')}
                >
                    Exercícios
                </Title>
                <ListContainer heightContainer={'65%'}>
                    <FlatList
                        data={exercises}
                        renderItem={renderItem}
                        keyExtractor={item => item.exerciseId}
                        contentContainerStyle={{
                            gap: 10
                        }}
                    />
                </ListContainer>

                <SelectedExerciseModal
                    visible={showModalExercise}
                    setShowModalExercise={setShowModalExercise}
                    navigation={navigation}
                />
            </Container>
        </Gradient>
    );
};
